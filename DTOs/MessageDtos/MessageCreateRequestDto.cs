namespace ClassRoomClone_App.Server.DTOs;

// Message DTOs
public class MessageCreateRequestDto
{
    public int SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string ParentType { get; set; } = null!;
    public int ParentId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsPrivate { get; set; }
}
