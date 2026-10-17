using Aero.Flights.Contracts.Flights;
using FluentValidation;

namespace Aero.Flights.Application.Flights.Routings;

public sealed class RoutingRequestValidator : AbstractValidator<OpsRoutingRequest>
{
    public RoutingRequestValidator() => RuleFor(x => x.AirportCode).NotEmpty().MaximumLength(4);
}
