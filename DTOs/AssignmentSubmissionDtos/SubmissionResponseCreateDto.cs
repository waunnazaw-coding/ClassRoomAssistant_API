namespace ClassRoomClone_App.Server.DTOs;


using Microsoft.AspNetCore.Http;

public class SubmissionResponseCreateDto
{
    public string ResponseType { get; set; } = null!; 
    public IFormFile? File { get; set; }              
    public string? FilePath { get; set; }            
    public string? Link { get; set; }
}


