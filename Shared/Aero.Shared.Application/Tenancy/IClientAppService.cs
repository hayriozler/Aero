namespace Aero.Shared.Application.Tenancy;

public interface IClientAppService
{
    Task<bool> VerifyAsync(string tenantId, string clientId, string clientSecret, CancellationToken ct);
    Task<string?> GetSecretAsync(string tenantId, string clientId, CancellationToken ct);
}
