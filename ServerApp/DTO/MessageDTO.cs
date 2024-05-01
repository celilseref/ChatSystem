using System;

namespace ServerApp.DTO
{
    public class MessageDto
{
    public int MessageId { get; set; }
    public string Text { get; set; }
    public DateTime DateSent { get; set; }
    public UserDto Sender { get; set; }
}
}