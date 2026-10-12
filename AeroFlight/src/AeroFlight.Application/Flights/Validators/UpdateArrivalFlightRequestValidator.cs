using AeroFlight.Contracts.Flights;
using FluentValidation;

namespace AeroFlight.Application.Flights.Validators;

public sealed class UpdateArrivalFlightRequestValidator : AbstractValidator<UpdateArrivalFlightRequest>
{
    public UpdateArrivalFlightRequestValidator()
    {
        RuleFor(x => x.OriginAirportCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.FlightTypeCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.FlightNumber).NotEmpty().MaximumLength(FlightNumberFormat.MaxLength)
            .Must(FlightNumberFormat.IsValid)
            .WithMessage("Flight number must be a code (max 3 chars) followed by a number (max 5 digits), e.g. \"TK 2563\" or \"SXS45862\".");
        RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.CallSign).MaximumLength(20);
        RuleFor(x => x.OperatingAirlineCode).MaximumLength(3);
        RuleFor(x => x.AircraftCode).MaximumLength(20);
        RuleFor(x => x.AircraftTypeCode).MaximumLength(10);
        RuleFor(x => x.TerminalCode).MaximumLength(10);
        RuleFor(x => x.HandlingAgentCode).MaximumLength(10);
        RuleFor(x => x.Comment).MaximumLength(250);
    }
}
