namespace Aero.Shared.Application.Managers;

public interface IClientAppManager
{
    Task<bool> VerifyAsync(string tenantId, string clientId, string clientSecret, CancellationToken ct);
    Task<string?> GetSecretAsync(string tenantId, string clientId, CancellationToken ct);
}
