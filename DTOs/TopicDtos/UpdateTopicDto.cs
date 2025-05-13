namespace ClassRoomClone_App.Server.DTOs;

public class UpdateTopicDto
{
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
}