namespace ClassRoomClone_App.Server.DTOs;

public class AssignmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int? Points { get; set; }
    public DateTime? CreatedAt { get; set; }
}