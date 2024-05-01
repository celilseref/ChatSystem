using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTO
{


   public class UserForProfileDto
{
    public string FullName { get; set; }
    public string UserImg { get; set; }
    public string Bio { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }

    public bool IsActive { get; set; }
}

}
