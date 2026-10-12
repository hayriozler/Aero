using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record AssignRoleCommand(int UserId, int RoleId);

public sealed class AssignRoleCommandHandler(IAeroFlightDbContext db) : ICommandHandler<AssignRoleCommand>
{
    public async Task HandleAsync(AssignRoleCommand command, CancellationToken ct)
    {
        var user = await db.UserSet.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is null) throw new KeyNotFoundException($"User {command.UserId} not found.");
        if (string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The admin user is protected and its roles cannot be changed.");

        var roleExists = await db.RoleSet.AnyAsync(r => r.Id == command.RoleId, ct);
        if (!roleExists) throw new KeyNotFoundException($"Role {command.RoleId} not found.");

        var alreadyAssigned = await db.UserRoleSet
            .AnyAsync(ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId, ct);
        if (alreadyAssigned) return;

        db.UserRoleSet.Add(UserRole.Create(command.UserId, command.RoleId));
        await db.SaveChangesAsync(ct);
    }
}
