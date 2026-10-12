using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BaggageService.Persistence;

/// <summary>
/// Design-time factory to ensure 'dotnet ef' can create AeroScanDataContext when running from CLI.
/// Reads connection string named "aeroscan" from appsettings.json or environment variables.
/// </summary>
public class AeroScanDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AeroScanDataContext>
{
    public AeroScanDataContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        var conn = config.GetConnectionString("aeroscan");
        if (string.IsNullOrEmpty(conn))
        {
            // fallback to environment variable AEROSCAN_CONNECTION
            conn = Environment.GetEnvironmentVariable("AEROSCAN_CONNECTION");
        }

        if (string.IsNullOrEmpty(conn))
            throw new InvalidOperationException("Connection string 'aeroscan' not found. Set it in appsettings.json or the AEROSCAN_CONNECTION environment variable.");

        var optionsBuilder = new DbContextOptionsBuilder<AeroScanDataContext>();
        optionsBuilder.UseNpgsql(conn);

        return new AeroScanDataContext(optionsBuilder.Options);
    }
}
