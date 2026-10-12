using Aero.Scanner.Shared.Requests;
using FluentValidation;

namespace Aero.Scanner.Api.Validators;

public sealed class SystemConfigurationRequestValidator : AbstractValidator<CreateSystemConfigurationRequest>
{
    public SystemConfigurationRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
    }
}