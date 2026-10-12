using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Roles;

public record DeleteRoleCommand(int Id);

public enum DeleteRoleResult { Deleted, NotFound, Protected }

public sealed class DeleteRoleCommandHandler(IAeroFlightDbContext db) : ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
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
