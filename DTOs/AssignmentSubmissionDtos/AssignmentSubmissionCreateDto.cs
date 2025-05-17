namespace ClassRoomClone_App.Server.DTOs;


public class AssignmentSubmissionCreateDto
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public string ResponseType { get; set; } = null!; 
    public IFormFile? File { get; set; }              
    //public string? FilePath { get; set; }            
    public string? Link { get; set; }
}
