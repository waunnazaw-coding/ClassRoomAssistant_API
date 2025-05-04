namespace ClassRoomClone_App.Server.DTOs;

public class SubmissionResponseCreateDto
{
    public string ResponseType { get; set; } = null!; // "File" or "Link"
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? Link { get; set; }
}
