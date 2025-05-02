namespace ClassRoomClone_App.Server.DTOs;

public class AttachmentDetailDto
{
    public int Id { get; set; }
    public string FileType { get; set; } = null!;
    public string? FileUrl { get; set; }
    public string? FilePath { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}