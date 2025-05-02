using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Grade
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int ClassWorkId { get; set; }

    public decimal Score { get; set; }

    public decimal MaxScore { get; set; }

    public int GradedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ClassWork ClassWork { get; set; } = null!;

    public virtual User GradedByNavigation { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
