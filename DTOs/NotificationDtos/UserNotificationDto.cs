namespace ClassRoomClone_App.Server.DTOs;

public class UserNotificationDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public int ReferenceId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string Details { get; set; } = null!;
}
