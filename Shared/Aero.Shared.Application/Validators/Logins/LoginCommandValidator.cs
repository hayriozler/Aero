using Aero.Shared.Application.Commands.Logins;
using FluentValidation;

namespace Aero.Shared.Application.Validators.Logins;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}