namespace AeroFlight.Application.Tenancy;

// Per-request resolved tenant; defaults to the fallback connection until Set.
public interface ITenantContext
{
    string TenantId { get; }
    string ConnectionString { get; }
    string HomeAirportCode { get; }
    bool IsResolved { get; }
    void Set(TenantInfo tenant);
}
