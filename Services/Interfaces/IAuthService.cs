using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

using System.Threading.Tasks;
using ClassRoomClone_App.Server.DTOs;
using Microsoft.AspNetCore.Identity.Data;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
    Task<UserResponseDto_> GetMeAsync(int userId);
    Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken);
}
