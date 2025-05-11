namespace ClassRoomClone_App.Server.DTOs;

public class ClassUpdateRequestDto
{
    public string Name { get; set; } = null!;
    public string Section { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Room { get; set; } = null!;
}