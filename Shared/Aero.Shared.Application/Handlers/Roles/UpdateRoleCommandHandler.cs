using Aero.Flights.Application.Common;
using Aero.Shared.Application.Commands.Roles;
using Aero.Shared.Contracts.Response;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;


public sealed class UpdateRoleCommandHandler(IDataStoreContext db) : ICommandHandler<UpdateRoleCommand, RoleResponse?>
{
    public async Task<RoleResponse?> HandleAsync(UpdateRoleCommand command, CancellationToken ct)
    {
        var role = await db.RoleSet.FirstOrDefaultAsync(r => r.Id == command.Id, ct);
        if (role is null) return null;

        role.Update(command.Name, command.Description);
        await db.SaveChangesAsync(ct);

        return new(
        role.Id, role.Name, role.Description,
        role.CreatedAt, role.CreatedBy, role.UpdatedAt, role.UpdatedBy);
    }
}
