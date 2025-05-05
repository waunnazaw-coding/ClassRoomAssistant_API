using System.Security.Claims;
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}