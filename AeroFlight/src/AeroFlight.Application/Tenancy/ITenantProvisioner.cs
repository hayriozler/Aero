namespace AeroFlight.Application.Tenancy;

// Encrypts the supplied plaintext connection string before persisting it to the catalog.
public interface ITenantProvisioner
{
    Task UpsertAsync(string id, string name, string connectionString, string homeAirportCode,
        bool isActive = true, CancellationToken ct = default);
}
