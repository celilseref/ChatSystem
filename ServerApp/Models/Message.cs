using System;

namespace ServerApp.models
{
    public class Message
    {
        public int MessageId { get; set; }
       public DateTime DateSent { get; set; }
    public int SenderId { get; set; }
    public User Sender { get; set; }
    public int? ReceiverId { get; set; }
    public User Receiver { get; set; }
    public int? GroupId { get; set; }
    public Group Group { get; set; }
    #nullable enable
     public string? FileType { get; set; }
      #nullable enable
     public string? FilePath { get; set; }
      public string? Text { get; set; }
  
    }
}