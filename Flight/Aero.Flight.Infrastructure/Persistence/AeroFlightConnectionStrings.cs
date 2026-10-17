using Npgsql;

namespace Aero.Flights.Infrastructure.Persistence;

public static class AeroFlightConnectionStrings
{
    public static string ForTenant(string rootConnectionString, string tenantId) =>
        new NpgsqlConnectionStringBuilder(rootConnectionString) { Database = $"{tenantId}_aodb" }.ConnectionString;
}

