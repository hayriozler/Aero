using Aero.Shared.Application.Commands.RolePermissions;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.RolePermissions;

public sealed class SetRolePermissionsCommandHandler(IDataStoreContext db)
    : ICommandHandler<SetRolePermissionsCommand>
{
    public async Task HandleAsync(SetRolePermissionsCommand command, CancellationToken ct)
    {
        var role = await db.Query<Role>().FirstOrDefaultAsync(r => r.Id == command.RoleName, ct);
        if (role is not null && string.Equals(role.Id, "Admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The Admin role is protected and its permissions cannot be changed.");

        var existing = await db.Query<RolePermission>().Where(rp => rp.RoleName == command.RoleName).ToListAsync(ct);
        foreach (var rp in existing)
            db.RemoveEntity(rp);

        //var valid = Permissions.All.Select(p => p.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        //foreach (var code in command.PermissionCodes.Distinct().Where(valid.Contains))
        //    db.AddEntity(RolePermission.Create(command.RoleName, code));

        await db.SaveChangesAsync(ct);
    }
}

