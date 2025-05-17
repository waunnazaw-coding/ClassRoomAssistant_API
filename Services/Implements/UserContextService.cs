using System.Security.Claims;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        // Try common claim types for user ID
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                          ?? user.FindFirst("sub")
                          ?? user.FindFirst("userId");

        if (userIdClaim == null)
            throw new Exception("User ID claim not found.");

        if (!int.TryParse(userIdClaim.Value, out int userId))
            throw new Exception("User ID claim is not a valid integer.");

        return userId;
    }


    public string GetCurrentUserEmail()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var emailClaim = user.FindFirst(ClaimTypes.Email);

        if (emailClaim == null)
            throw new Exception("Email claim not found.");

        return emailClaim.Value;
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}