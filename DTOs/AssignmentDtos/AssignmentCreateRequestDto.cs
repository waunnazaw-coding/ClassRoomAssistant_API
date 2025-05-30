namespace ClassRoomClone_App.Server.DTOs;

// Models/AssignmentCreateRequest.cs
public class AssignmentCreateRequest
{
    public int ClassId { get; set; }
    public bool CreateNewTopic { get; set; }
    public string? NewTopicTitle { get; set; }
    public int? SelectedTopicId { get; set; }
    public string AssignmentTitle { get; set; } = null!;
    public string? Instructions { get; set; }
    public int? Points { get; set; }
    public DateTime? DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
    public List<int>? StudentIds { get; set; } // null or empty means all students
    public List<AttachmentDto>? Attachments { get; set; }
}

public class AttachmentDto
{
    public string FileType { get; set; } = null!; // "Drive", "YouTube", "Upload", "Link"
    public string? FileUrl { get; set; }
    public string? FilePath { get; set; }
    
    public IFormFile? FileUpload { get; set; }
}

// Models/AssignmentCreateResponse.cs
public class AssignmentCreateResponse
{
    public int AssignmentId { get; set; }
    public int ClassWorkId { get; set; }
    public int? TopicId { get; set; }
}
