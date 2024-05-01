using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.DTO;
using ServerApp.models;

namespace ServerApp.Data
{
public class GroupRepository : IGroupRepository
{
    private readonly ChatContext _context;

    public GroupRepository(ChatContext context)
    {
        _context = context;
    }

       public async Task<IEnumerable<GroupListDto>> GetAllGroupsAsync()
{
    return await _context.Set<Group>()
                                .Select(g => new GroupListDto { GroupId = g.GroupId, GroupName = g.GroupName })
                                .ToListAsync();
}

public async Task<IEnumerable<GroupWithMembershipDto>> GetAllGroupsWithMembership(int userId)
{
    var groups = await _context.Groups
        .Select(g => new GroupWithMembershipDto
        {
            GroupId = g.GroupId,
            GroupName = g.GroupName,
            IsMember = g.GroupUsers.Any(gu => gu.UserId == userId)
        })
        .ToListAsync();

    return groups;
}

       public async Task<List<GroupChatInfoDto>> GetGroupsByGroupNameAsync(string groupName)
{
    var groups = await _context.Groups
        .Where(g => g.GroupName.ToLower().Contains(groupName.ToLower()))
        .Select(g => new GroupChatInfoDto
        {
            GroupName = g.GroupName,
            GroupImg = g.GroupImg,
        })
        .ToListAsync();

    return groups;
}


        public async Task<List<GroupUserDto>> GetGroupUsersByGroupName(string groupName)
    {
        var group = await _context.Groups.Include(g => g.GroupUsers)
                                         .ThenInclude(gu => gu.User)
                                         .FirstOrDefaultAsync(g => g.GroupName == groupName);

        if (group == null)
            return null;

        var groupUsersDto = group.GroupUsers.Select(gu => new GroupUserDto
        {
            UserImg = gu.User.userImg,
            Bio = gu.User.Bio,
            FullName = gu.User.FullName
        }).ToList();

        return groupUsersDto;
    }


    }
}