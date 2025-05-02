namespace ClassRoomClone_App.Server.DTOs;

public class MaterialDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}


