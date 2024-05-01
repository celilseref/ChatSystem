using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.DTO;
using ServerApp.models;

namespace ServerApp.Data
{
public class MessageRepository : IMessageRepository
{
    private readonly ChatContext _context;

    public MessageRepository(ChatContext context)
    {
        _context = context;
    }

public List<object> GetMessagesBetweenUsers(string senderUserName, string receiverUserName, int page)
{
    var count = 10;
    var messages = _context.Messages
        .Where(m => (m.Sender.UserName == senderUserName && m.Receiver.UserName == receiverUserName)
                || (m.Sender.UserName == receiverUserName && m.Receiver.UserName == senderUserName))
        .OrderByDescending(m => m.DateSent)
          .Skip((page - 1) * count)// count - 10 kadar mesajı atla
        .Take(count) // Son 10 mesajı al
        .Select(m => new {
            Text = m.Text,
            DateSent = m.DateSent,
            SenderUserName = m.Sender.UserName,
            SenderFullName = m.Sender.FullName,
            Type = (m.Sender.UserName == senderUserName) ? 1 : 0,
            FilePath=m.FilePath,
            FileType=m.FileType
        })
        .OrderBy(m => m.DateSent) // Mesajları en eskiden yeniye sırala
        .ToList<object>();

    return messages;
}



public List<object> GetGroupMessages(string groupName,int page)
{
    var count = 10;
    var messages = _context.Messages
        .Where(m => m.Group.GroupName == groupName)
        .OrderByDescending(m => m.DateSent)
        .Skip((page - 1) * count) // count - 10 kadar mesajı atla
        .Take(count)
        .Select(m => new {
            Text = m.Text,
            DateSent = m.DateSent,
            SenderUserName = m.Sender.UserName,
            SenderFullName = m.Sender.FullName,
              FilePath=m.FilePath,
            FileType=m.FileType

        })
        .OrderBy(m => m.DateSent)
        .ToList<object>();

    return messages;
}



public List<ConversationDto> GetConversationsForUser(string userName)
{
var conversations = new List<ConversationDto>();
var user = _context.Users
    .Include(u => u.ReceivedMessages)
    .ThenInclude(m => m.Sender)
    .SingleOrDefault(u => u.UserName == userName);

if (user == null)
{
    return conversations;
}

var messages = user.ReceivedMessages.OrderByDescending(m => m.DateSent).ToList();
var contacts = messages.GroupBy(m => m.Sender).ToList();

foreach (var contact in contacts)
{
    var lastMessage = contact.FirstOrDefault();

    var conversation = new ConversationDto
    {
         userImg = lastMessage.Sender.userImg,
        ContactName = lastMessage.Sender.UserName,
        LastMessage = lastMessage.Text,
        LastMessageDate = lastMessage.DateSent,
        fullName = lastMessage.Sender.FullName,
        IsActive=lastMessage.Sender.IsActive

    };

    conversations.Add(conversation);
}

return conversations;
}

   
public List<ChatGroupDto> GetLastMessagesForUser(int userId)
    {
        var messages = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Group)
            .Where(m => m.ReceiverId == userId || (m.GroupId.HasValue && m.Group.GroupUsers.Any(gu => gu.UserId == userId)))
            .GroupBy(m => m.GroupId)
            .SelectMany(g => g.OrderByDescending(m => m.DateSent).Take(1))
            .Select(m => new ChatGroupDto
            {
                SenderFullName = m.Sender.FullName,
                SenderUserName = m.Sender.UserName,
                Text = m.Text,
                DateSent = m.DateSent,
                GroupName = m.Group.GroupName,
                GroupImg=m.Group.GroupImg
            })
            .ToList();

        return messages;
    }

public async Task<List<ChatGroupDto>> GetLastMessagesByUserInGroups(int userId)
{
    var messages = await _context.Messages
        .Include(m => m.Sender)
        .Include(m => m.Group)
        .Where(m => m.Group.GroupUsers.Any(gu => gu.UserId == userId))
        .ToListAsync();

    if (messages == null || messages.Count == 0)
        return null;

    var messageDtos = messages.GroupBy(m => m.GroupId)
        .Select(g => g.OrderByDescending(m => m.DateSent).FirstOrDefault())
        .AsEnumerable()
        .Select(m => new ChatGroupDto
        {
            SenderUserName = m.Sender.UserName,
            SenderFullName = m.Sender.FullName,
            DateSent = m.DateSent,
            Text = m.Text,
            GroupName = m.Group.GroupName
        }).ToList();

    return messageDtos;
}




}
}