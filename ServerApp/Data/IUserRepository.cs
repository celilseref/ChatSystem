using System.Collections.Generic;
using System.Threading.Tasks;
using ServerApp.DTO;
using ServerApp.models;

namespace ServerApp.Data
{
    public interface IUserRepository
    {
         IEnumerable<User> GetAllUsers();

         Task<UserForProfileDto> GetUserByUserNameAsync(string userName);
         Task<UserFoRChatInfoDTO> GetUserByUserForChat(string userName);
    }
}
