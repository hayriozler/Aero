using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using AeroFlight.Domain.Identity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record CreateUserCommand(string Username, string Password, string? RealName);

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public sealed class CreateUserCommandHandler(IAeroFlightDbContext db, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var exists = await db.UserSet.AnyAsync(u => u.Username == command.Username, ct);
        if (exists) throw new InvalidOperationException($"Username '{command.Username}' is already taken.");

        var user = User.Create(command.Username, passwordHasher.Hash(command.Password), command.RealName);
        db.UserSet.Add(user);
        await db.SaveChangesAsync(ct);

        return UserResponseFactory.From(user, []);
    }
}
