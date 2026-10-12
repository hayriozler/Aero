namespace AeroFlight.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateToken(int userId, string username, string tenant, IEnumerable<string> roles);

    // For a service principal (see ClientApp) — not a User row, so no numeric user id/username claim.
    (string Token, DateTimeOffset ExpiresAt) GenerateServiceToken(string clientId, string tenant, IEnumerable<string> roles);
}
