using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class AircraftRequestValidator : AbstractValidator<AircraftRequest>
{
    public AircraftRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.AircraftTypeCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.AirlineCode).MaximumLength(3);
        RuleFor(x => x.Owner).MaximumLength(100);
        RuleFor(x => x.SizeCategory).MaximumLength(5);
        RuleFor(x => x.NoiseCategory).MaximumLength(10);
    }
}
