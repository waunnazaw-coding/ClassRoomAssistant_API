namespace ClassRoomClone_App.Server.DTOs;

public class MaterialResponseDto
{
    public int MaterialId { get; set; }
    public string Title { get; set; } = null!;
    public int ClassWorkId { get; set; }
    public List<AttachmentResponseDto> Attachments { get; set; } = new();
}