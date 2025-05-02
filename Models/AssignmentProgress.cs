using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class AssignmentProgress
{
    public int AssignmentId { get; set; }

    public int AssignedCount { get; set; }

    public int? TurnedInCount { get; set; }

    public int? GradedCount { get; set; }

    public int? ReviewedBy { get; set; }

    public int? GradedBy { get; set; }

    public DateTime? LastReviewedAt { get; set; }

    public DateTime? LastGradedAt { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User? GradedByNavigation { get; set; }

    public virtual User? ReviewedByNavigation { get; set; }
}
