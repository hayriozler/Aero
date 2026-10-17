using Aero.Shared.Application.Commands.Roles;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;

public sealed class CreateRoleCommandHandler(IDataStoreContext db) : ICommandHandler<CreateRoleCommand, RoleResponse>
{
    public async Task<RoleResponse> HandleAsync(CreateRoleCommand command, CancellationToken ct)
    {
        var exists = await db.RoleSet.AnyAsync(r => r.Id == command.Name, ct);
        if (exists) throw new InvalidOperationException($"Role '{command.Name}' already exists.");

        var role = Role.Create(command.Name, command.Description);
        db.AddEntity(role);
        await db.SaveChangesAsync(ct);

        return new(
        role.Id, role.Description,
        role.CreatedAt, role.CreatedBy, role.UpdatedAt, role.UpdatedBy);
    }
}


