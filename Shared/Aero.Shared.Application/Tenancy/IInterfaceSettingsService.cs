namespace Aero.Shared.Application.Tenancy;

public interface IInterfaceSettingsService
{
    Task<string?> FindAsync(string tenantId, string key, CancellationToken ct = default);
    Task SetAsync(string tenantId, string key, string value, CancellationToken ct = default);
    void Invalidate();
}
