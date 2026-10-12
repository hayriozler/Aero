using AeroFids.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AeroFlight.Migrations;

internal sealed class FidsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FidsDbContext>
{
    public FidsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FidsDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=aeroflight_design;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations"))
            .Options;
        return new FidsDbContext(options);
    }
}
