using Contracts.Requests;
using FluentValidation;

namespace BaggageService.Validators;

public sealed class CreateHandheldTerminalRequestValidator : AbstractValidator<CreateHandheldTerminalRequest>
{
    public CreateHandheldTerminalRequestValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}

public sealed class UpdateHandheldTerminalRequestValidator : AbstractValidator<UpdateHandheldTerminalRequest>
{
    public UpdateHandheldTerminalRequestValidator() => RuleFor(x => x.Name).NotEmpty();
}

public sealed class AssignHandheldTerminalRequestValidator : AbstractValidator<AssignHandheldTerminalRequest>
{
    public AssignHandheldTerminalRequestValidator() => RuleFor(x => x.HandlingCompanyCode).NotEmpty();
}
