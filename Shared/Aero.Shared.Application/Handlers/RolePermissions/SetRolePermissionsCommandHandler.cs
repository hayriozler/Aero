using Aero.Flights.Application.Common;
using Aero.Shared.Application.Commands.RolePermissions;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;

namespace Aero.Shared.Application.Handlers.RolePermissions;

public sealed class SetRolePermissionsCommandHandler(IDataStoreContext db)
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

