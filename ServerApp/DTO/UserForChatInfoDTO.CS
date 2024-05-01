using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTO
{


   public class UserFoRChatInfoDTO
{
    public string FullName { get; set; }
    public string UserImg { get; set; }
    public string UserName { get; set; }

    public bool IsActive { get; set; }
   
}

}
