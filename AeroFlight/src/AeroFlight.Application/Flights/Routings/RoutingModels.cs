using AeroFlight.Contracts.Flights;
using FluentValidation;

namespace AeroFlight.Application.Flights.Routings;

public sealed class RoutingRequestValidator : AbstractValidator<OpsRoutingRequest>
{
    public RoutingRequestValidator() => RuleFor(x => x.AirportCode).NotEmpty().MaximumLength(4);
}
