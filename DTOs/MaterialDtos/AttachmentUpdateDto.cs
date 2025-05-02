namespace ClassRoomClone_App.Server.DTOs;

public class AttachmentUpdateDto
{
    public int? Id { get; set; }  // Null means new attachment
    public string FileType { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? FilePath { get; set; }
}