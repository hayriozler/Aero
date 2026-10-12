namespace AeroFlight.Application.Tenancy;

// Generic per-tenant, per-interface settings (see TenantInterfaceSetting) — e.g. FIDS's own
// database connection string, stored explicitly in the tenant catalog rather than derived at
// runtime from the AODB connection string.
public interface IInterfaceSettingsService
{
    Task<string?> FindAsync(string tenantId, string key, CancellationToken ct = default);
    Task SetAsync(string tenantId, string key, string value, CancellationToken ct = default);
    void Invalidate();
}
