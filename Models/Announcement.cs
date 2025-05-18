using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Announcement
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;
}
