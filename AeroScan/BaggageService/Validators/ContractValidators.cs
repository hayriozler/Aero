using Contracts.Requests;
using FluentValidation;

namespace BaggageService.Validators;

public sealed class CreateAirlineHandlingContractRequestValidator : AbstractValidator<CreateAirlineHandlingContractRequest>
{
    public CreateAirlineHandlingContractRequestValidator()
    {
        RuleFor(x => x.AirlineCode).NotEmpty();
        RuleFor(x => x.HandlingCompanyCode).NotEmpty();
        RuleFor(x => x.ValidTo)
            .GreaterThan(x => x.ValidFrom)
            .WithMessage("ValidTo must be after ValidFrom.");
    }
}

public sealed class UpdateAirlineHandlingContractRequestValidator : AbstractValidator<UpdateAirlineHandlingContractRequest>
{
    public UpdateAirlineHandlingContractRequestValidator()
    {
        RuleFor(x => x.AirlineCode).NotEmpty();
        RuleFor(x => x.HandlingCompanyCode).NotEmpty();
        RuleFor(x => x.ValidTo)
            .GreaterThan(x => x.ValidFrom)
            .WithMessage("ValidTo must be after ValidFrom.");
    }
}
