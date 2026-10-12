using Microsoft.AspNetCore.Authorization;

namespace AeroFlight.Api.Infrastructure.Authorization;

public sealed class PermissionRequirement(string code) : IAuthorizationRequirement
{
    public string Code { get; } = code;
}
