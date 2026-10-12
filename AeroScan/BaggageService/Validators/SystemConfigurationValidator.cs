using Contracts.Requests;
using FluentValidation;

namespace BaggageService.Validators;

public sealed class SystemConfigurationRequestValidator : AbstractValidator<CreateSystemConfigurationRequest>
{
    public SystemConfigurationRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
    }
}