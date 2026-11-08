using Aero.Flights.Contracts.SeasonalFlightSchedules;
using FluentValidation;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Validators;

public sealed class SeasonalFlightScheduleArrivalFlightRequestValidator : AbstractValidator<SeasonalFlightScheduleArrivalFlightRequest>
{
    public SeasonalFlightScheduleArrivalFlightRequestValidator()
    {
        RuleFor(x => x.ScheduleSeasonCode).NotEmpty().MaximumLength(15);
        RuleFor(x => x.AirlineCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.FlightNumber).NotEmpty().MaximumLength(10);
        RuleFor(x => x.OriginAirportCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.PeriodEnd).GreaterThanOrEqualTo(x => x.PeriodBegin);
    }
}
