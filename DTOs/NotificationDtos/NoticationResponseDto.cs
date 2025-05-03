namespace ClassRoomClone_App.Server.DTOs;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = null!;
    public int ReferenceId { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
}