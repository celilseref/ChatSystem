using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.models;
using Microsoft.AspNetCore.Identity;

namespace ServerApp.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class GroupController 
{
  private readonly IGroupRepository _groupRepository;
    public GroupController(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

  
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
    {
        var groups = await _groupRepository.GetAllGroupsAsync();
        return new OkObjectResult(groups);
    }

[HttpGet("list/{userId}")]
public async Task<ActionResult<IEnumerable<GroupWithMembershipDto>>> GetAllGroupsWithMembership(int userId)
{
    var groups = await _groupRepository.GetAllGroupsWithMembership(userId);
    return new OkObjectResult(groups);
}


    [HttpGet("{groupName}")]
    public async Task<ActionResult<List<GroupChatInfoDto>>> GetGroupsByGroupName(string groupName)
    {
        var groups = await _groupRepository.GetGroupsByGroupNameAsync(groupName);

        if (groups == null || groups.Count == 0)
        {
           return new NotFoundResult();

        }

        return new OkObjectResult(groups);
    }

[HttpGet("{groupName}/users")]
    public async Task<IActionResult> GetGroupUsers(string groupName)
    {
        var groupUsersDto = await _groupRepository.GetGroupUsersByGroupName(groupName);

        if (groupUsersDto == null)
            return new NotFoundResult();

        return new OkObjectResult(groupUsersDto);
    }


}

}
