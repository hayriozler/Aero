namespace Aero.Shared.Application.Tenancy;

public interface ITenantProvisioner
{
    Task UpsertAsync(string id, string name, string connectionString, string homeAirportCode,
        bool isActive = true, CancellationToken ct = default);
}