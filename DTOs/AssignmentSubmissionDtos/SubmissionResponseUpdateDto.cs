namespace ClassRoomClone_App.Server.DTOs;

public class SubmissionResponseUpdateDto
{
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? Link { get; set; }
}