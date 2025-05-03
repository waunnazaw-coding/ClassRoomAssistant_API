namespace ClassRoomClone_App.Server.Models;

public class UserNotificationRawDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public int ReferenceId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? AnnouncementTitle { get; set; }
    public string? MessageContent { get; set; }
    public string? AssignmentTitle { get; set; }
    public string? MaterialTitle { get; set; }
}
