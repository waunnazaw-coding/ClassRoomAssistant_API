namespace ClassRoomClone_App.Server.DTOs;

public class MaterialCreateRequestDto
{
    public int ClassId { get; set; }
    public int? TopicId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public List<AttachmentCreateDto> Attachments { get; set; } = new();
    public int CreatedBy { get; set; }
}
