namespace ClassRoomClone_App.Server.DTOs.AuthDtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
