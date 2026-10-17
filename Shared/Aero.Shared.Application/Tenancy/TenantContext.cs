using Aero.Shared.Domain;

namespace Aero.Shared.Application.Tenancy;

public sealed class TenantContext : ITenantContext
{
    private string? _connectionString;

    public string TenantId { get; private set; } = string.Empty;

    public string ConnectionString => _connectionString
        ?? throw new InvalidOperationException("No tenant has been resolved for the current scope.");

    public string HomeAirportCode { get; private set; } = string.Empty;

    public bool IsResolved => _connectionString is not null;

    public void Set(TenantInfo tenant)
    {
        TenantId = tenant.Id;
        _connectionString = tenant.ConnectionString;
        HomeAirportCode = tenant.HomeAirportCode;
    }
}
