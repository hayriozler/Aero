using Npgsql;

namespace AeroFids.Infrastructure.Persistence;

public static class FidsConnectionStrings
{
    public static string ForTenant(string rootConnectionString, string tenantId) =>
        new NpgsqlConnectionStringBuilder(rootConnectionString) { Database = $"{tenantId}_fids" }.ConnectionString;
}
