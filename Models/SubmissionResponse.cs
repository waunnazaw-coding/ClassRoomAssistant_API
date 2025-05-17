using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class SubmissionResponse
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    public string FileType { get; set; } = null!;

    public string? FilePath { get; set; }

    public string? Link { get; set; }

    public string? FileName { get; set; }   

    public DateTime UploadedAt { get; set; }

    public virtual AssignmentSubmission Submission { get; set; } = null!;
}
