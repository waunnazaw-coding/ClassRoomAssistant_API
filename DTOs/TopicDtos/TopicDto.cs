namespace ClassRoomClone_App.Server.DTOs;

public class TopicDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int ClassId { get; set; }
    public DateTime? CreatedAt { get; set; }
}