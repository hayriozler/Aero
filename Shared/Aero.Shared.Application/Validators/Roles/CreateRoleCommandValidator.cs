using Aero.Shared.Application.Commands.Roles;
using FluentValidation;

namespace Aero.Shared.Application.Validators.Roles;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}
