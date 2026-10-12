using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record UpdateUserCommand(int Id, string? RealName);

public sealed class UpdateUserCommandHandler(IAeroFlightDbContext db) : ICommandHandler<UpdateUserCommand, UserResponse?>
{
    public async Task<UserResponse?> HandleAsync(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await db.UserSet
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == command.Id, ct);

        if (user is null) return null;

        user.UpdateProfile(command.RealName);
        await db.SaveChangesAsync(ct);

        var roles = user.UserRoles.Where(ur => ur.Role is not null).Select(ur => ur.Role!.Name).ToList();
        return UserResponseFactory.From(user, roles);
    }
}
