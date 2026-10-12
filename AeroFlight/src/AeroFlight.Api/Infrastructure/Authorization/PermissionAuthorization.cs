using System.Security.Claims;
using AeroFlight.Application.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AeroFlight.Api.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IUserPermissionService permissions)
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
