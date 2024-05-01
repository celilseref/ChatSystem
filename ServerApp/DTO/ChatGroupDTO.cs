using System;

namespace ServerApp.DTO
{
public class ChatGroupDto
{ public string SenderFullName { get; set; }
public string SenderUserName { get; set; }
public string Text { get; set; }
public DateTime DateSent { get; set; }

public string GroupImg { get; set; }
public string GroupName { get; set; }
}
}