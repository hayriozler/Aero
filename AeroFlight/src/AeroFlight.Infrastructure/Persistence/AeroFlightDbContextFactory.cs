using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Infrastructure.Persistence;
    public interface IAeroFlightDbContextFactory
    {
        AeroFlightDbContext Create(string connectionString);
    }
public sealed class AeroFlightDbContextFactory : IAeroFlightDbContextFactory
{
    public AeroFlightDbContext Create(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<AeroFlightDbContext>()
               .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations"));
        #if DEBUG
            builder.EnableSensitiveDataLogging().EnableDetailedErrors();
        #endif
        return new AeroFlightDbContext(builder.Options);

    }
}
