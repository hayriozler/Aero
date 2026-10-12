using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record RemoveRoleCommand(int UserId, int RoleId);

public sealed class RemoveRoleCommandHandler(IAeroFlightDbContext db) : ICommandHandler<RemoveRoleCommand>
{
    public async Task HandleAsync(RemoveRoleCommand command, CancellationToken ct)
    {
        var user = await db.UserSet.FirstOrDefaultAsync(u => u.Id == command.UserId, ct);
        if (user is not null && string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The admin user is protected and its roles cannot be changed.");

        var userRole = await db.UserRoleSet
            .FirstOrDefaultAsync(ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId, ct);

        if (userRole is null) return;

        db.UserRoleSet.Remove(userRole);
        await db.SaveChangesAsync(ct);
    }
}
