using Aero.Flights.Application.Common;
using Aero.Shared.Application.Commands.Roles;
using Aero.Shared.Contracts.Response;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;

namespace Aero.Shared.Application.Handlers.Roles;

public sealed class CreateRoleCommandHandler(IDataStoreContext db) : ICommandHandler<CreateRoleCommand, RoleResponse>
{
    public async Task<RoleResponse> HandleAsync(CreateRoleCommand command, CancellationToken ct)
    {
        var exists = await db.RoleSet.AnyAsync(r => r.Name == command.Name, ct);
        if (exists) throw new InvalidOperationException($"Role '{command.Name}' already exists.");

        var role = Role.Create(command.Name, command.Description);
        db.RoleSet.Add(role);
        await db.SaveChangesAsync(ct);

        return RoleResponseFactory.From(role);
    }
}


