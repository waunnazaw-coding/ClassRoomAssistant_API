using ClassRoomClone_App.Server.Repositories.Interfaces;

namespace ClassRoomClone_App.Server.CustomAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

public class ClassRoleAuthorizationHandler : AuthorizationHandler<ClassRoleRequirement, int>
{
    private readonly IClassParticipantsRepository _classParticipantsRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClassRoleAuthorizationHandler(
        IClassParticipantsRepository classParticipantsRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _classParticipantsRepository = classParticipantsRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ClassRoleRequirement requirement,
        int classId)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            context.Fail();
            return;
        }

        // Query user role in the class
        var userRole = await _classParticipantsRepository.GetUserRoleInClassAsync(userId, classId);
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
