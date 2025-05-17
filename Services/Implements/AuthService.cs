using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.DTOs.AuthDtos;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        var existingUser = await _userRepository.GetByEmailAsync(model.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        var tokens = await _jwtService.GenerateTokensAsync(user);

        return MapToAuthResponseDto(tokens);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        var user = await _userRepository.GetByEmailAsync(model.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var tokens = await _jwtService.GenerateTokensAsync(user);
        return MapToAuthResponseDto(tokens);
    }

    public async Task<UserResponseDto_> GetMeAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserResponseDto_
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Profile = user.Profile
        };
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

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new SecurityTokenException("Invalid or expired refresh token.");

        var tokens = await _jwtService.GenerateTokensAsync(user);
        return MapToAuthResponseDto(tokens);
    }

    private AuthResponseDto MapToAuthResponseDto(AuthResponse tokens)
    {
        return new AuthResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiration = tokens.AccessTokenExpiration
            // Optionally map other fields if needed
        };
    }
}
