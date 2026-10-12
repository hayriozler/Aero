using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Roles;

public sealed record GetRolePermissionsQuery(int RoleId);

public sealed record SetRolePermissionsCommand(int RoleId, IReadOnlyList<string> PermissionCodes);

public sealed class GetRolePermissionsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetRolePermissionsQuery, IReadOnlyList<string>>
{
    public async Task<IReadOnlyList<string>> HandleAsync(GetRolePermissionsQuery query, CancellationToken ct)
        => await db.Query<RolePermission>()
            .Where(rp => rp.RoleId == query.RoleId)
            .Select(rp => rp.PermissionCode)
            .ToListAsync(ct);
}

public sealed class SetRolePermissionsCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<SetRolePermissionsCommand>
{
    public async Task HandleAsync(SetRolePermissionsCommand command, CancellationToken ct)
    {
        var role = await db.Query<Role>().FirstOrDefaultAsync(r => r.Id == command.RoleId, ct);
        if (role is not null && string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The Admin role is protected and its permissions cannot be changed.");

        var existing = await db.Query<RolePermission>().Where(rp => rp.RoleId == command.RoleId).ToListAsync(ct);
        foreach (var rp in existing)
            db.Remove(rp);

        var valid = Permissions.All.Select(p => p.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var code in command.PermissionCodes.Distinct().Where(valid.Contains))
            db.Add(RolePermission.Create(command.RoleId, code));

        await db.SaveChangesAsync(ct);
    }
}
