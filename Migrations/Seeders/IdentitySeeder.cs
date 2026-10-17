using Aero.Migrations.Options;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Identity;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AeroFlight.Migrations.Seeders;

internal sealed partial class IdentitySeeder(
    AeroFlightDbContext db,
    IPasswordHasher passwordHasher,
    IOptions<SeedOptions> opts,
    ILogger<IdentitySeeder> logger)
{
    public async Task SeedAsync(CancellationToken ct)
    {
        var options = opts.Value;

        var adminRoleId = await EnsureRoleAsync("Admin", "Full system access — all operations permitted", ct);
        var viewAllRoleId = await EnsureRoleAsync("ViewAll", "Read access to all resources", ct);
        var writeRoleId = await EnsureRoleAsync("WriteMetadata", "Create and update Metadata data", ct);
        var fidsServiceRoleId = await EnsureRoleAsync("FidsService", "Aero.Fids' own client-credentials principal — flight read access only", ct);

        await EnsurePermissionCatalogAsync(ct);
        await AssignPermissionsAsync(adminRoleId, Permissions.All.Select(p => p.Code), ct);
        await AssignPermissionsAsync(viewAllRoleId, [Permissions.MetadataView, Permissions.FlightView, Permissions.SeasonalFlightScheduleView], ct);
        await AssignPermissionsAsync(writeRoleId, [Permissions.MetadataView, Permissions.MetadataCreate, Permissions.MetadataUpdate], ct);
        await AssignPermissionsAsync(fidsServiceRoleId, [Permissions.FlightView], ct);

        await SeedDemoUserAsync(options.AdminUsername, options.AdminPassword, options.AdminRealName, [adminRoleId], ct);
        await SeedDemoUserAsync(options.NormalUsername, options.NormalPassword, options.NormalRealName, [viewAllRoleId, writeRoleId], ct);
        await SeedDemoUserAsync(options.ReadOnlyUsername, options.ReadOnlyPassword, options.ReadOnlyRealName, [viewAllRoleId], ct);
    }

    private async Task SeedDemoUserAsync(
        string username, string password, string? realName, IReadOnlyList<int> roleIds, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            LogPasswordNotConfigured(logger, username);
            return;
        }

        var userId = await EnsureUserAsync(username, password, realName, ct);
        foreach (var roleId in roleIds)
            await EnsureUserRoleAsync(userId, roleId, username, ct);
    }

    private async Task<int> EnsureRoleAsync(string name, string? description, CancellationToken ct)
    {
        var existing = await db.RoleSet.FirstOrDefaultAsync(r => r.Name == name, ct);
        if (existing is not null)
            return existing.Id;

        var role = Role.Create(name, description);
        db.RoleSet.Add(role);
        await db.SaveChangesAsync(ct);
        LogRoleCreated(logger, name, role.Id);
        return role.Id;
    }

    private async Task<int> EnsureUserAsync(string username, string password, string? realName, CancellationToken ct)
    {
        var existing = await db.UserSet.FirstOrDefaultAsync(u => u.Username == username, ct);
        if (existing is not null)
            return existing.Id;

        var user = User.Create(username, passwordHasher.Hash(password), realName);
        db.UserSet.Add(user);
        await db.SaveChangesAsync(ct);
        LogUserCreated(logger, username, user.Id);
        return user.Id;
    }

    private async Task EnsureUserRoleAsync(int userId, int roleId, string username, CancellationToken ct)
    {
        var exists = await db.UserRoleSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
        if (exists) return;

        db.UserRoleSet.Add(UserRole.Create(userId, roleId));
        await db.SaveChangesAsync(ct);
        LogRoleAssigned(logger, username);
    }

    private async Task EnsurePermissionCatalogAsync(CancellationToken ct)
    {
        var existing = await db.PermissionSet.Select(p => p.Code).ToHashSetAsync(ct);
        var toAdd = Permissions.All.Where(p => !existing.Contains(p.Code))
            .Select(p => Permission.Create(p.Code, p.Description)).ToList();
        if (toAdd.Count == 0) return;

        db.PermissionSet.AddRange(toAdd);
        await db.SaveChangesAsync(ct);
    }

    private async Task AssignPermissionsAsync(int roleId, IEnumerable<string> permissionCodes, CancellationToken ct)
    {
        var existing = await db.RolePermissionSet.Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionCode).ToHashSetAsync(ct);
        var toAdd = permissionCodes.Where(c => !existing.Contains(c))
            .Select(c => RolePermission.Create(roleId, c)).ToList();
        if (toAdd.Count == 0) return;

        db.RolePermissionSet.AddRange(toAdd);
        await db.SaveChangesAsync(ct);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Seed password for '{Username}' is not configured. Skipping user seed.")]
    private static partial void LogPasswordNotConfigured(ILogger logger, string username);

    [LoggerMessage(Level = LogLevel.Information, Message = "Role '{Name}' created with id {Id}.")]
    private static partial void LogRoleCreated(ILogger logger, string name, int id);

    [LoggerMessage(Level = LogLevel.Information, Message = "User '{Username}' created with id {Id}.")]
    private static partial void LogUserCreated(ILogger logger, string username, int id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Role assigned to '{Username}'.")]
    private static partial void LogRoleAssigned(ILogger logger, string username);
}
