namespace ClassRoomClone_App.Server.DTOs;

public class AnnouncementResponseDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Announcements { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
}
