using System.Security.Claims;
using ClassRoomClone_App.Server.DTOs.AuthDtos;
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IJwtService
{
    Task<AuthResponse> GenerateTokensAsync(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    //string GenerateAccessToken(User user);
    //string GenerateRefreshToken();
    //ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}