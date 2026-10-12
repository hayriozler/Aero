using Contracts.Requests;
using FluentValidation;

namespace BaggageService.Validators;

public sealed class CreateAirlineClassMapRequestValidator : AbstractValidator<CreateAirlineClassMapRequest>
{
    public CreateAirlineClassMapRequestValidator()
    {
        RuleFor(x => x.AirlineCode).NotEmpty().MaximumLength(6);
        RuleFor(x => x.SourceClass).NotNull()
            .WithMessage("Source class must be a single character (max 1).");
        RuleFor(x => x.TargetClass).NotNull()
            .WithMessage("Target class must be a single character (max 1).");
    }
}

public sealed class UpdateAirlineClassMapRequestValidator : AbstractValidator<UpdateAirlineClassMapRequest>
{
    public UpdateAirlineClassMapRequestValidator() => RuleFor(x => x.TargetClass).NotNull()
            .WithMessage("Target class must be a single character (max 1).");
}

public sealed class CreateResourceStatusMapRequestValidator : AbstractValidator<CreateResourceStatusMapRequest>
{
    public CreateResourceStatusMapRequestValidator()
    {
        RuleFor(x => x.SourceResourceName).NotEmpty().MaximumLength(20);
        RuleFor(x => x.SourceResourceStatus).NotEmpty().MaximumLength(20);
        RuleFor(x => x.TargetResourceStatus).NotEmpty().MaximumLength(1)
            .WithMessage("Target status must be a single character (max 1).");
    }
}

public sealed class UpdateResourceStatusMapRequestValidator : AbstractValidator<UpdateResourceStatusMapRequest>
{
    public UpdateResourceStatusMapRequestValidator() => RuleFor(x => x.TargetResourceStatus).NotEmpty().MaximumLength(1)
            .WithMessage("Target status must be a single character (max 1).");
}
