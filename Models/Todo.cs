using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Todo
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ClassWorkId { get; set; }

    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public bool? IsMissing { get; set; }

    public virtual ClassWork ClassWork { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
