namespace ClassRoomClone_App.Server.DTOs;

public class MessageDto
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string CommentMessage { get; set; } = null!;
    public bool? IsPrivate { get; set; }
    public DateTime? CommentCreatedAt { get; set; }
}