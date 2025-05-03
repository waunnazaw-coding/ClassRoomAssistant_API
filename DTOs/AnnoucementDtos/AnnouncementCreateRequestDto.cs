namespace ClassRoomClone_App.Server.DTOs;

public class AnnouncementCreateRequestDto
{
    public int ClassId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
}





