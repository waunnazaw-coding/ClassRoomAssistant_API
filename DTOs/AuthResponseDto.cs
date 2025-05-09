namespace ClassRoomClone_App.Server.DTOs;

public class AuthResponseDto
{
    public int Id { get; set; }
    
    public string Email { get; set; }
    
    public string Name { get; set; }
    
    public string ? Profile { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}