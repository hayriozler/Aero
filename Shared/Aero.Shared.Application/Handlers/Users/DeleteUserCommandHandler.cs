using Aero.Shared.Application.Commands.Users;
using Aero.Shared.Domain.Enums;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Users;

public sealed class DeleteUserCommandHandler(IDataStoreContext db) : ICommandHandler<DeleteUserCommand, DeleteUserResult>
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

