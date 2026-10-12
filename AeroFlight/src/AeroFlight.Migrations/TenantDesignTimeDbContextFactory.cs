using AeroFlight.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AeroFlight.Migrations;

internal sealed class TenantDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=Tenant;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations"))
            .Options;
        return new TenantDbContext(options);
    }
}