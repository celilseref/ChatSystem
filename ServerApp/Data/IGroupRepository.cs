using System.Collections.Generic;

using System.Threading.Tasks;
using ServerApp.DTO;
using ServerApp.models;

namespace ServerApp.Data
{
    public interface IGroupRepository
    {
        public Task<IEnumerable<GroupListDto>> GetAllGroupsAsync();

        Task<List<GroupChatInfoDto>> GetGroupsByGroupNameAsync(string groupName);

        public Task<IEnumerable<GroupWithMembershipDto>> GetAllGroupsWithMembership(int userId); 
        Task<List<GroupUserDto>> GetGroupUsersByGroupName(string groupName);

    }
     
}