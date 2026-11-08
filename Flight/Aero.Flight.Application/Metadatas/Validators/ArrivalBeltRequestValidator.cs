using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class ArrivalBeltRequestValidator : AbstractValidator<ArrivalBeltRequest>
{
    public ArrivalBeltRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}
