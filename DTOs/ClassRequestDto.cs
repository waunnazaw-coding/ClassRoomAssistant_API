namespace ClassRoomClone_App.Server.DTOs;

// Request DTO used for creating or updating a class
public class ClassRequestDto
{
    public string Name { get; set; } = null!;
    public string Section { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Room { get; set; } = null!;
}