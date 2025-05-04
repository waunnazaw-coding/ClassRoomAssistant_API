using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class AssignmentSubmission
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }

    public int StudentId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User Student { get; set; } = null!;

    public virtual ICollection<SubmissionResponse> SubmissionResponses { get; set; } = new List<SubmissionResponse>();
}
