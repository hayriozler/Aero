using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class AirlineRequestValidator : AbstractValidator<AirlineRequest>
{
    public AirlineRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.IataCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.IcaoCode).MaximumLength(4);
        RuleFor(x => x.Callsign).MaximumLength(50);
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.DisplayName).MaximumLength(200);
        RuleFor(x => x.ParentAirlineCode).MaximumLength(10);
        RuleFor(x => x.CountryCode).MaximumLength(3);
    }
}
