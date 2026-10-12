namespace AeroFlight.Application.Tenancy;

// A service principal's client credentials (see ClientApp), stored encrypted (not hashed) in the
// tenant catalog DB so a legitimate caller — AeroFids included — can decrypt and use its own secret
// directly, with no copy of it ever needed in that caller's own config.
public interface IClientAppService
{
    Task<bool> VerifyAsync(string tenantId, string clientId, string clientSecret, CancellationToken ct);
    Task<string?> GetSecretAsync(string tenantId, string clientId, CancellationToken ct);
}
