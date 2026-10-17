using Aero.Flights.Application.Common;
using Aero.Shared.Application.Commands.Roles;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;

public enum DeleteRoleResult { Deleted, NotFound, Protected }

public sealed class DeleteRoleCommandHandler(IDataStoreContext db) : ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
{
    public async Task<DeleteRoleResult> HandleAsync(DeleteRoleCommand command, CancellationToken ct)
    {
        var role = await db.RoleSet.FirstOrDefaultAsync(r => r.Id == command.Id, ct);
        if (role is null) return DeleteRoleResult.NotFound;

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            return DeleteRoleResult.Protected;

        db.RoleSet.Remove(role);
        await db.SaveChangesAsync(ct);
        return DeleteRoleResult.Deleted;
    }
}
