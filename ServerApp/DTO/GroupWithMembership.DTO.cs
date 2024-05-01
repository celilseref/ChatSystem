using System;

namespace ServerApp.DTO
{public class GroupWithMembershipDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public bool IsMember { get; set; }
}

}