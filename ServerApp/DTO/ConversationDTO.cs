using System;

namespace ServerApp.DTO
{
    public class ConversationDto
{
   public string userImg { get; set; }
    public string ContactName { get; set; }
    public string LastMessage { get; set; }

    public bool IsActive { get; set; }
    public DateTime LastMessageDate { get; set; }

    public string fullName { get; set; }
}
}