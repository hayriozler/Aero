using Aero.Shared.Domain.Identity;

namespace Aero.Shared.Domain;

public sealed class JwtOptions
{
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 480;
    public User User { get; init; } = null!;
    public string TenantId { get; init; } = string.Empty;
}

