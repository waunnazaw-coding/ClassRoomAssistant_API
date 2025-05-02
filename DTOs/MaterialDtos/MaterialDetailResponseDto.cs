namespace ClassRoomClone_App.Server.DTOs;

public class MaterialDetailResponseDto
{
    public int MaterialId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ClassWorkId { get; set; }
    public List<AttachmentDetailDto> Attachments { get; set; } = new();
}


