using ClassRoomClone_App.Server.Repositories.Interfaces;

namespace ClassRoomClone_App.Server.CustomAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

public class ClassRoleHandler : AuthorizationHandler<ClassRoleRequirement>
{
    private readonly IClassParticipantsRepository _classParticipantRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClassRoleHandler(IClassParticipantsRepository classParticipantRepository, IHttpContextAccessor httpContextAccessor)
    {
        _classParticipantRepository = classParticipantRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClassRoleRequirement requirement)
    {
        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            context.Fail();
            return;
        }
        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            context.Fail();
            return;
        }

        // Get ClassId from route or query string (adjust as needed)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        // Try to get classId from route data
        if (!httpContext.Request.RouteValues.TryGetValue("classId", out var classIdObj) ||
            !int.TryParse(classIdObj?.ToString(), out int classId))
        {
            // Optionally, try query string or fail
            context.Fail();
            return;
        }

        // Get user role in class
        var userRole = await _classParticipantRepository.GetUserRoleInClassAsync(userId, classId);
        if (userRole == null)
        {
            context.Fail();
            return;
        }

        // Check if user role is allowed
        if (requirement.AllowedRoles.Contains(userRole))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
