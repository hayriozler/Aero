using AeroFids.Infrastructure.Identity;
using AeroFids.Infrastructure.Persistence;
using AeroFlight.Application.Interfaces;
using AeroFlight.Migrations.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AeroFlight.Migrations.Seeders;

// FIDS has its own, separate Identity — this only ever creates the one Admin account, reusing
// the same seeded credentials and password hasher as AeroFlight's admin user.
internal sealed partial class FidsIdentitySeeder(
    FidsDbContext db, IPasswordHasher passwordHasher, IOptions<SeedOptions> opts, ILogger<FidsIdentitySeeder> logger)
{
    public async Task SeedAsync(CancellationToken ct)
    {
        var options = opts.Value;
        if (string.IsNullOrWhiteSpace(options.AdminPassword))
        {
            LogPasswordNotConfigured(logger, options.AdminUsername);
            return;
        }

        var existing = await db.FidsUserSet.FirstOrDefaultAsync(u => u.Username == options.AdminUsername, ct);
        if (existing is not null)
            return;

        var user = FidsUser.Create(options.AdminUsername, passwordHasher.Hash(options.AdminPassword), FidsRoles.Admin, DateTimeOffset.UtcNow);
        db.FidsUserSet.Add(user);
        await db.SaveChangesAsync(ct);
        LogUserCreated(logger, options.AdminUsername);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Seed password for '{Username}' is not configured. Skipping FIDS admin seed.")]
    private static partial void LogPasswordNotConfigured(ILogger logger, string username);

    [LoggerMessage(Level = LogLevel.Information, Message = "FIDS admin user '{Username}' created.")]
    private static partial void LogUserCreated(ILogger logger, string username);
}
