using Aero.Shared.Application.Commands.UserRoles;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.UserRoles;

public sealed class AssignUserRoleCommandHandler(IDataStoreContext db) : ICommandHandler<AssignUserRoleCommand>
{
    public async Task HandleAsync(AssignUserRoleCommand command, CancellationToken ct)
    {
        var user = await db.UserSet.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is null) throw new KeyNotFoundException($"User {command.UserId} not found.");
        if (string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The admin user is protected and its roles cannot be changed.");

        var roleExists = await db.RoleSet.AnyAsync(r => r.Id == command.RoleName, ct);
        if (!roleExists) throw new KeyNotFoundException($"Role {command.RoleName} not found.");

        var alreadyAssigned = await db.UserRoleSet
            .AnyAsync(ur => ur.UserId == command.UserId && ur.RoleName == command.RoleName, ct);
        if (alreadyAssigned) return;

        db.UserRoleSet.Add(UserRole.Create(command.UserId, command.RoleName));
        await db.SaveChangesAsync(ct);
    }
}
