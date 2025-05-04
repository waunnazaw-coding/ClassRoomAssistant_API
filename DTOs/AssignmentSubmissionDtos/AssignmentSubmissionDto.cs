namespace ClassRoomClone_App.Server.DTOs;

public class AssignmentSubmissionDto
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<SubmissionResponseDto> SubmissionResponses { get; set; } = new();
}