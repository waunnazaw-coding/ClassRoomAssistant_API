namespace ClassRoomClone_App.Server.DTOs;

public class MaterialUpdateRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public List<AttachmentUpdateDto> Attachments { get; set; } = new();
}

