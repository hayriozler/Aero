using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class AircraftTypeRequestValidator : AbstractValidator<AircraftTypeRequest>
{
    public AircraftTypeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.IataCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.IcaoCode).MaximumLength(4);
        RuleFor(x => x.SizeCategory).MaximumLength(1);
        RuleFor(x => x.NoiseCategory).MaximumLength(10);
        RuleFor(x => x.Manufacturer).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
    }
}
