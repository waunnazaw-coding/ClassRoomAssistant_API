namespace ClassRoomClone_App.Server.DTOs;

public class SubmissionResponseDto
{
    public int Id { get; set; }
    public string ResponseType { get; set; } = null!;
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? Link { get; set; }
    public DateTime UploadedAt { get; set; }
}


