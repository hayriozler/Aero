using Contracts.Requests;
using FluentValidation;

namespace BaggageService.Validators;

public sealed class CreateContainerRequestValidator : AbstractValidator<CreateContainerRequest>
{
    public CreateContainerRequestValidator()
    {
        RuleFor(x => x.FlightId).GreaterThan(0);
        RuleFor(x => x.ContainerCode).NotEmpty();
        RuleFor(x => x.ContainerTypeCode).NotEmpty();
        RuleFor(x => x.ContainerStatusCode).NotEmpty();
        RuleFor(x => x.ContainerClassCode).NotEmpty();
        RuleFor(x => x.ContainerDestination).NotEmpty();
    }
}

public sealed class UpdateContainerRequestValidator : AbstractValidator<UpdateContainerRequest>
{
    public UpdateContainerRequestValidator()
    {
        RuleFor(x => x.ContainerCode).NotEmpty();
        RuleFor(x => x.ContainerTypeCode).NotEmpty();
        RuleFor(x => x.ContainerStatusCode).NotEmpty();
        RuleFor(x => x.ContainerClassCode).NotEmpty();
        RuleFor(x => x.ContainerDestination).NotEmpty();
    }
}

public sealed class CreateContainerTypeRequestValidator : AbstractValidator<CreateContainerTypeRequest>
{
    public CreateContainerTypeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(1)
            .WithMessage("Type code must be a single character (max 1).");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(20);
    }
}

public sealed class UpdateContainerTypeRequestValidator : AbstractValidator<UpdateContainerTypeRequest>
{
    public UpdateContainerTypeRequestValidator() => RuleFor(x => x.Description).NotEmpty().MaximumLength(20);
}
public sealed class CreateContainerClassRequestValidator : AbstractValidator<CreateContainerClassRequest>
{
    public CreateContainerClassRequestValidator()
    {
        RuleFor(x => x.ContainerTypeCode).NotEmpty().MaximumLength(1)
            .WithMessage("Type code must be a single character (max 1).");
        RuleFor(x => x.ClassCode).NotEmpty().MaximumLength(1)
            .WithMessage("Class code must be a single character (max 1).");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(20);
    }
}

public sealed class UpdateContainerClassRequestValidator : AbstractValidator<UpdateContainerClassRequest>
{
    public UpdateContainerClassRequestValidator() => RuleFor(x => x.Description).NotEmpty().MaximumLength(20);
}
