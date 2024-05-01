using System.Collections.Generic;

namespace ServerApp.models
{
    public class Group
    {
         public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupImg { get; set; }
    public string  GroupInfo { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<GroupUser> GroupUsers { get; set; }
    }
}