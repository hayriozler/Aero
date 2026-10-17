namespace Aero.Shared.Application.Managers;

public interface IInterfaceSettingsManager
{
    Task<string?> FindAsync(string tenantId, string key, CancellationToken ct = default);
    Task SetAsync(string tenantId, string key, string value, CancellationToken ct = default);
    void Invalidate();
}
