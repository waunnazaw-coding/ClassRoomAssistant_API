namespace ClassRoomClone_App.Server.DTOs;

public class AssignmentResponseDto
{
    public int Id { get; set; }
    public int ClassWorkId { get; set; }
    public string Title { get; set; } = null!;
    public string? Instructions { get; set; }
    public int? Points { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? AllowLateSubmission { get; set; }
}