using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.DTOs;

public class ClassParticipantsDto
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int? UserId { get; set; }

    public string Role { get; set; } = null!;

    public bool? IsOwner { get; set; }

    //public int? AddedBy { get; set; }

    public DateTime? AddedAt { get; set; }
    
    //public bool IsDeleted { get; set; } = false;
 
    
    public User? User { get; set; }
    
}