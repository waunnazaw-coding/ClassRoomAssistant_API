using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ClassRoomClone_App.Server.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepo, IJwtService jwtService)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }
        
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already registered.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepo.AddAsync(user);
            return await GenerateTokensAsync(createdUser);
        }
        
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            return await GenerateTokensAsync(user);
        }
        
        public async Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) 
                throw new ArgumentNullException(nameof(accessToken));
            if (string.IsNullOrWhiteSpace(refreshToken)) 
                throw new ArgumentNullException(nameof(refreshToken));

            var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                throw new SecurityTokenException("Invalid access token.");

            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
                throw new SecurityTokenException("Invalid token claims.");

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new SecurityTokenException("Invalid or expired refresh token.");

            return await GenerateTokensAsync(user);
        }
        
        public async Task LogoutAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userRepo.UpdateAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokensAsync(User user)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days
            await _userRepo.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Name = user.Name,
                Email = user.Email,
                Profile = user.Profile,
                Id = user.Id
            };
        }
    }
}
