namespace ClassRoomClone_App.Server.DTOs;

public class ClassParticipantResponseDto
{
    public int? UserId { get; set; }         // Added UserId for reference
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool? IsOwner { get; set; }
    public DateTime? AddedAt { get; set; }
}