namespace ClassRoomClone_App.Server.DTOs;


public class AssignmentSubmissionCreateDto
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public List<SubmissionResponseCreateDto> Responses { get; set; } = new();
}
