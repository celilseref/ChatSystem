using System;
namespace ServerApp.DTO
{
public class ChatInfoMessageDto
{
  public string SenderUserName { get; set; }
    public string SenderFullName { get; set; }
    public DateTime DateSent { get; set; }
    public string Text { get; set; }
    public string GroupOrReceiverName { get; set; }

    public string GroupOrReceiverImg { get; set; }
}
}