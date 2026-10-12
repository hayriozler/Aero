using Microsoft.EntityFrameworkCore;

namespace AeroFids.Infrastructure.Persistence;

public interface IFidsDbContextFactory
{
    FidsDbContext Create(string connectionString);
}

public sealed class FidsDbContextFactory : IFidsDbContextFactory
{
    public FidsDbContext Create(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<FidsDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations"));
        #if DEBUG
            builder.EnableSensitiveDataLogging().EnableDetailedErrors();
        #endif
        return new FidsDbContext(builder.Options);
    }
}
