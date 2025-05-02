using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Attachment
{
    public int Id { get; set; }

    public int ReferenceId { get; set; }

    public string? ReferenceType { get; set; }

    public string? FileType { get; set; }

    public string? FileUrl { get; set; }

    public string? FilePath { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
