namespace ClassRoomClone_App.Server.DTOs;

public class AnnouncementWithMessagesDto
{
    public int AnnouncementId { get; set; }
    public int ClassId { get; set; }
    public string Title { get; set; } = null!;
    public string AnnouncementMessage { get; set; } = null!;
    public DateTime? AnnouncementCreatedAt { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
}