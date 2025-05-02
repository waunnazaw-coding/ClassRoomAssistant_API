namespace ClassRoomClone_App.Server.DTOs;

public class AttachmentResponseDto
{
    public int Id { get; set; }
    public string FileType { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
}