using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTO
{


    public class UserForRegisterDTO
    {
        [Required]
        public string UserName { get; set; }
         
         public string FullName { get; set; }

        public string Email { get; set; }

        [Required]

        public string Password { get; set; }
    }
}
