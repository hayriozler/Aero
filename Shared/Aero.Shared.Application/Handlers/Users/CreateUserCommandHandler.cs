using Aero.Shared.Application.Commands.Users;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;
using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
namespace Aero.Shared.Application.Handlers.Users;


public sealed class CreateUserCommandHandler(IDataStoreContext db, IPasswordHasherService passwordHasher)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var exists = await db.UserSet.AnyAsync(u => u.Username == command.Username, ct);
        if (exists) throw new InvalidOperationException($"Username '{command.Username}' is already taken.");

        var user = User.Create(command.Username, passwordHasher.Hash(command.Password), command.RealName);
        db.UserSet.Add(user);
        await db.SaveChangesAsync(ct);

        return new(user.Id, user.Username, user.RealName, user.IsActive, user.ExpirationDate,
        [], user.CreatedAt, user.CreatedBy, user.UpdatedAt, user.UpdatedBy);
    }
}
