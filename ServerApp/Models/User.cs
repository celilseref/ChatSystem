using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ServerApp.models
{
    public class User:IdentityUser<int>
    {
    public int UserId { get; set; }

   public string FullName { get; set; }

     public string userImg { get; set; }

     public string Bio { get; set; }

     public bool IsActive { get; set; }
    
    public ICollection<Message> ReceivedMessages { get; set; }
    public ICollection<GroupUser> GroupUsers { get; set; }
    }
}