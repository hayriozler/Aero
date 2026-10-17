using Aero.Shared.Domain;

namespace Aero.Shared.Application.Tenancy;

public interface ITenantStore
{
    Task<TenantInfo?> FindAsync(string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<TenantInfo>> GetActiveAsync(CancellationToken ct = default);
    void Invalidate();
}