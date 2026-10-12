namespace AeroFlight.Application.Tenancy;

// Connection string is already decrypted here; the store handles decryption internally.
public sealed record TenantInfo(string Id, string Name, string ConnectionString, string HomeAirportCode, bool IsActive);

public interface ITenantStore
{
    Task<TenantInfo?> FindAsync(string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<TenantInfo>> GetActiveAsync(CancellationToken ct = default);
    void Invalidate();
}
