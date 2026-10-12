using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using AeroFlight.Domain.Identity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Roles;

public record CreateRoleCommand(string Name, string? Description);

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class CreateRoleCommandHandler(IAeroFlightDbContext db) : ICommandHandler<CreateRoleCommand, RoleResponse>
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
