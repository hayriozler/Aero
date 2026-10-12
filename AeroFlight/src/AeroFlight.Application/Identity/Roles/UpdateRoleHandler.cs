using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Roles;

public record UpdateRoleCommand(int Id, string Name, string? Description);

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class UpdateRoleCommandHandler(IAeroFlightDbContext db) : ICommandHandler<UpdateRoleCommand, RoleResponse?>
{
    public async Task<RoleResponse?> HandleAsync(UpdateRoleCommand command, CancellationToken ct)
    {
        var role = await db.RoleSet.FirstOrDefaultAsync(r => r.Id == command.Id, ct);
        if (role is null) return null;

        role.Update(command.Name, command.Description);
        await db.SaveChangesAsync(ct);

        return RoleResponseFactory.From(role);
    }
}
