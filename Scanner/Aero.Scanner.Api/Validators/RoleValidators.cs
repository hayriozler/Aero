using Aero.Scanner.Shared.Requests;
using FluentValidation;

namespace Aero.Scanner.Api.Validators;

public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty();
    }
}

public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator() => RuleFor(x => x.DisplayName).NotEmpty();
}
