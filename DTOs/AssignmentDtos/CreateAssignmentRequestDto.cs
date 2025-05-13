namespace ClassRoomClone_App.Server.DTOs;

// Request DTOs
public class CreateAssignmentRequestDto
{
    public int ClassId { get; set; }
    public int? TopicId { get; set; }
    public string Title { get; set; } = null!;
    public string? Instructions { get; set; }
    public int? Points { get; set; }
    public DateTime? DueDate { get; set; }
    public List<AttachmentCreateDto> Attachments { get; set; } = new();
}