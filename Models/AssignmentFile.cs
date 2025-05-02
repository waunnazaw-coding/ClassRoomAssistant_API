using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class AssignmentFile
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }

    public int StudentId { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
