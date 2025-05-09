namespace ClassRoomClone_App.Server.CustomAuthorization;

using Microsoft.AspNetCore.Authorization;

public class ClassRoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public ClassRoleRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
