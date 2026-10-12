using AeroFlight.Contracts.Flights;
using FluentValidation;

namespace AeroFlight.Application.Flights.ParkPositiions;

public sealed class ParkPositionRequestValidator : AbstractValidator<OpsParkPositionRequest>
{
    public ParkPositionRequestValidator()
    {
        // Code is optional — operators add a placeholder resource first and assign the real code later.
        RuleFor(x => x.Code).MaximumLength(10);
    }
}
