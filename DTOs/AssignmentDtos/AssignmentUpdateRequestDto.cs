namespace ClassRoomClone_App.Server.DTOs;

public class AssignmentUpdateRequestDto
{
    public string Title { get; set; } = null!;
    public string? Instructions { get; set; }
    public int? Points { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? AllowLateSubmission { get; set; }
}