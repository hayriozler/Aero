using Aero.Shared.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Aero.Shared.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler(IUserPermissionService permissions)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        if (roles.Count == 0)
            return;

        var granted = await permissions.GetPermissionsAsync(roles, CancellationToken.None);
        if (granted.Contains(requirement.Code))
            context.Succeed(requirement);
    }
}
