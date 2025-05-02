using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class ClassParticipant
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int? UserId { get; set; }

    public string Role { get; set; } = null!;

    public bool? IsOwner { get; set; }

    public int? AddedBy { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual User? AddedByNavigation { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User? User { get; set; }
}
