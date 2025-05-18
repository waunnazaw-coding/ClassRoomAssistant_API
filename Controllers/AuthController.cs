using System.Security.Claims;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ClassRoomClone_App.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) { _authService = authService; }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }


    [Authorize]
    [HttpGet("get-me")]
    public async Task<IActionResult> GetMe()
    {
        // Try to get the user ID claim (adjust claim type as per your token)
        var userIdClaim = User.FindFirst("userId")
                       ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("sub");// fallback to "sub" if needed

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _authService.GetMeAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception as needed
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { ex.Message });
        }
    }
    

}
