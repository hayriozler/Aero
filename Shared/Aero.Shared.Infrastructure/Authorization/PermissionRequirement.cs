using Microsoft.AspNetCore.Authorization;

namespace Aero.Shared.Infrastructure.Authorization;

public sealed class PermissionRequirement(string code) : IAuthorizationRequirement
{
    public string Code { get; } = code;
}
