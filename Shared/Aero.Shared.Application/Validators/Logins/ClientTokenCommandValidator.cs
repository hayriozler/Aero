using Aero.Shared.Application.Commands.Logins;
using FluentValidation;

namespace Aero.Shared.Application.Validators.Logins;

public sealed class ClientTokenCommandValidator : AbstractValidator<ClientTokenCommand>
{
    public ClientTokenCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.ClientSecret).NotEmpty();
    }
}
