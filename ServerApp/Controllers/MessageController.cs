using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.models;

namespace ServerApp.Controllers
{
[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

      private readonly IHubContext<ChatHub> _hubContext;
    private readonly ChatContext _dbContext;

        public object Context { get; private set; }

        public MessageController(IMessageRepository messageRepository, IUserRepository userRepository,IHubContext<ChatHub> hubContext, ChatContext dbContext)
    {
        _messageRepository = messageRepository;
        _userRepository=userRepository;
        _hubContext = hubContext;
        _dbContext = dbContext;

    }

    // Get messages between two users
    [HttpGet]
public IActionResult GetMessages(string senderUserName, string receiverUserName, int? page)
{
    if (page == null)
    {
        page = 1;
    }
    var messages = _messageRepository.GetMessagesBetweenUsers(senderUserName, receiverUserName, page.Value);
        
    if (messages == null)
        return NotFound();

    return Ok(messages);
}
[HttpGet("group/{groupName}")]
public IActionResult GetMessagesFromGroup(string groupName,int? page)
{
    if (page == null)
    {
        page = 1;
    }
   
    var messages = _messageRepository.GetGroupMessages(groupName,page.Value);

    return Ok(messages);
}


   [HttpGet("last")]
    public IActionResult GetLastMessagesForUser(string userName)
    {
        var conversations = _messageRepository.GetConversationsForUser(userName);
        return Ok(conversations);
    }

    [HttpGet("recentgroup/{userId}")]
public async Task<ActionResult<IEnumerable<ChatGroupDto>>> GetLastMessagesByUserInGroups(int userId)
{
    var lastMessages = await _messageRepository.GetLastMessagesByUserInGroups(userId);
    if (lastMessages == null || lastMessages.Count == 0)
        return NotFound();
    return Ok(lastMessages);
}

[HttpPost]
[Route("user")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> SendFile([FromForm] string receiverUserName,[FromForm] string senderUserName, [FromForm] IFormFile file)
{
    try
    {
        Console.WriteLine("SendFile methodu çalıştı");

        // Get sender's UserName and ConnectionId
        var senderUserNam = senderUserName;
        var senderConnectionId = HttpContext.Connection.Id;
        var DateSent = DateTime.Now;

        // Get receiver's ConnectionId from ConnectedUsers dictionary
        var receiverConnectionId = ChatHub.ConnectedUsers.FirstOrDefault(x => x.Value == receiverUserName).Key;
        if (receiverConnectionId == null)
        {
            // Receiver not found
            return BadRequest($"User {receiverUserName} not found");
        }

        // Save file to disk
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName);
            var randomName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", randomName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save message to database
            var sender = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == senderUserNam);
            var receiver = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == receiverUserName);
            if (sender == null || receiver == null)
            {
                // User not found in database
                return BadRequest("User not found in database");
            }
            var newMessage = new Message
            {
                Text = null,
                DateSent = DateSent,
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                FileType = file.ContentType,
                FilePath = randomName
            };
            await _dbContext.Messages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();

            // Send message to receiver
            await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveFile", senderUserNam, newMessage.DateSent, newMessage.FileType, newMessage.FilePath);

            Console.WriteLine("Dosya başarıyla yüklendi ve mesaj gönderildi");
            return Ok(newMessage);
        }
        else
        {
            // File not found
            return BadRequest("File not found");
        }
    }
    catch (Exception ex)
    {
        // Handle exception
        Console.WriteLine("Dosya yüklenirken bir hata oluştu: " + ex.Message);
        return BadRequest("Failed to upload file: " + ex.Message);
    }
}


[HttpPost]
[Route("group")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> SendFileToGroup([FromForm] string groupName, [FromForm] string senderUserName, [FromForm] IFormFile file)
{
    try
    {
        Console.WriteLine("SendFileToGroup methodu çalıştı");

        // Get sender's UserName and ConnectionId
        var senderUserNam = senderUserName;
        var senderConnectionId = HttpContext.Connection.Id;
        var DateSent = DateTime.Now;

        // Get group by groupName from database
        var group = await _dbContext.Groups.SingleOrDefaultAsync(g => g.GroupName == groupName);

        if (group == null)
        {
            // Group not found
            return BadRequest($"Group {groupName} not found");
        }

        // Save file to disk
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName);
            var randomName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", randomName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save message to database
            var sender = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == senderUserNam);

            if (sender == null)
            {
                // User not found in database
                return BadRequest("User not found in database");
            }

            var newMessage = new Message
            {
                Text = null,
                DateSent = DateSent,
                SenderId = sender.Id,
                GroupId = group.GroupId,
                FileType = file.ContentType,
                FilePath = randomName
            };

            await _dbContext.Messages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();

            // Send message to all clients in the group
            await _hubContext.Clients.Group(group.GroupName).SendAsync("ReceiveFile", senderUserNam, newMessage.DateSent, newMessage.FileType, newMessage.FilePath);

            Console.WriteLine("Dosya başarıyla yüklendi ve mesaj gönderildi");
            return Ok(newMessage);
        }
        else
        {
            // File not found
            return BadRequest("File not found");
        }
    }
    catch (Exception ex)
    {
        // Handle exception
        Console.WriteLine("Dosya yüklenirken bir hata oluştu: " + ex.Message);
        return BadRequest("Failed to upload file: " + ex.Message);
    }
}


[HttpGet]
[Route("api/videos/{fileName}")]
public IActionResult GetVideo(string fileName)
{
    var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);
    var fileInfo = new FileInfo(videoPath);

    var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    var contentType = "video/mp4";

    long videoLength = fileInfo.Length;

    // Range isteği için gerekli bilgileri oku
    var rangeHeader = Request.Headers["Range"].ToString();
   if (!string.IsNullOrEmpty(rangeHeader))
{
    // range isteği var ise, sadece istenilen aralık kadar byte gönderilecek
    // response headerları hazırla
    var range = RangeHeaderValue.Parse(rangeHeader);
    long start = range.Ranges.First().From ?? 0; // From özelliğinin null olması durumunda 0 olarak ayarla
    long end = range.Ranges.First().To ?? fileInfo.Length - 1; // To özelliğinin null olması durumunda dosya boyutundan 1 eksik olarak ayarla
    var length = end - start + 1;
    stream.Seek(start, SeekOrigin.Begin);
    videoLength = length;
    Response.StatusCode = 206;
    Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileInfo.Length}");
    Response.Headers.Add("Content-Length", length.ToString());
}
else
{
    // range isteği yok ise, tam dosya gönderilecek
    // response headerları hazırla
    Response.Headers.Add("Content-Length", fileInfo.Length.ToString());
}


    // Gönderilen veri miktarını ve yüzdesini hesapla
    double percent = (double)videoLength / fileInfo.Length * 100;
    Console.WriteLine($"Gönderilen bellek miktarı: {videoLength} bytes (%{percent:F2} dosyanın tamamı)");

    return File(stream, contentType);
}


}

}
