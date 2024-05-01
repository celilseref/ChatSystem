using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTO
{
    public class UserForUpdateDTO
    {
        public string FullName { get; set; }

         public string userImg { get; set; }

          public string Bio { get; set; }

           public string Email { get; set; }
      
    }
}