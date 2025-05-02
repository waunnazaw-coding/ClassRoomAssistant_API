namespace ClassRoomClone_App.Server.DTOs;

public class AssignmentWithStatusDto
{
    public int AssignmentId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = null!;
    public string? Status { get; set; }
}
