using Aero.Shared.Domain;

namespace Aero.Shared.Application;

public interface ITenantContext
{
    string TenantId { get; }
    string ConnectionString { get; }
    string HomeAirportCode { get; }
    bool IsResolved { get; }
    void Set(TenantInfo tenant);
}
