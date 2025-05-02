namespace ClassRoomClone_App.Server.DTOs;

public class CreateTopicDto
{
    public string Title { get; set; } = null!;
    public int ClassId { get; set; }
}