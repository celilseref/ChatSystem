using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerApp.DTO;
using ServerApp.models;

namespace ServerApp.Data
{
    public interface IMessageRepository
    {
   List<object> GetMessagesBetweenUsers(string senderUserName, string receiverUserName,int page);

    List<ConversationDto> GetConversationsForUser(string userName);

    public List<object> GetGroupMessages(string groupName,int page);

    public Task<List<ChatGroupDto>> GetLastMessagesByUserInGroups(int userId);

    public List<ChatGroupDto> GetLastMessagesForUser(int userId);

      // Task<List<ChatInfoMessageDto>> GetLastMessagesByUserIdAsync(int userId);

      //  Task<List<ChatInfoMessageDto>> GetLastMessagesByUserId(int userId);

     

    }
    
}