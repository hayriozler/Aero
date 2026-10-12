using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record DeleteUserCommand(int Id);

public enum DeleteUserResult { Deleted, NotFound, Protected }

public sealed class DeleteUserCommandHandler(IAeroFlightDbContext db) : ICommandHandler<DeleteUserCommand, DeleteUserResult>
{
    public async Task<DeleteUserResult> HandleAsync(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await db.UserSet.FirstOrDefaultAsync(u => u.Id == command.Id, ct);
        if (user is null) return DeleteUserResult.NotFound;

        if (string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase))
            return DeleteUserResult.Protected;

        db.UserSet.Remove(user);
        await db.SaveChangesAsync(ct);
        return DeleteUserResult.Deleted;
    }
}
