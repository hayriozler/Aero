using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class HandlingServiceRequestValidator : AbstractValidator<HandlingServiceRequest>
{
    public HandlingServiceRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}
