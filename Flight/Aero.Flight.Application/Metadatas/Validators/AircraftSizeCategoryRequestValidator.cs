using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class AircraftSizeCategoryRequestValidator : AbstractValidator<AircraftSizeCategoryRequest>
{
    public AircraftSizeCategoryRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
    }
}