using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AeroFlight.Migrations;

internal sealed class AirportDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AeroFlightDbContext>
{
    public AeroFlightDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AeroFlightDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=aeroflight_design;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations"))
            .Options;
        return new AeroFlightDbContext(options);
    }
}
