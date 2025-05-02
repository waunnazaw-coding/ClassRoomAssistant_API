using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Assignment
{
    public int Id { get; set; }

    public int ClassWorkId { get; set; }

    public string Title { get; set; } = null!;

    public string? Instructions { get; set; }

    public int? Points { get; set; }

    public DateTime? DueDate { get; set; }

    public bool? AllowLateSubmission { get; set; }

    public virtual ICollection<AssignmentFile> AssignmentFiles { get; set; } = new List<AssignmentFile>();

    public virtual AssignmentProgress? AssignmentProgress { get; set; }

    public virtual ClassWork ClassWork { get; set; } = null!;
}
