using Aero.Shared.Application.Commands.UserRoles;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.UserRoles;

public sealed class RemoveUserRoleCommandHandler(IDataStoreContext db) : ICommandHandler<RemoveUserRoleCommand>
{
    public async Task HandleAsync(RemoveUserRoleCommand command, CancellationToken ct)
    {
        var user = await db.UserSet.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is not null && string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The admin user is protected and its roles cannot be changed.");

        var userRole = await db.UserRoleSet
            .FirstOrDefaultAsync(ur => ur.UserId == command.UserId && ur.RoleName == command.RoleName, ct);

        if (userRole is null) return;

        db.UserRoleSet.Remove(userRole);
        await db.SaveChangesAsync(ct);
    }
}

