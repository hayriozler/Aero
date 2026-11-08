using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class CreateSeasonalFlightScheduleArrivalFlightCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<CreateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse>
{
    public async Task<SeasonalFlightScheduleArrivalFlightResponse> HandleAsync(CreateSeasonalFlightScheduleArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = Build(SeasonalFlightScheduleArrivalFlight.Create(
            command.Request.ScheduleSeasonCode, command.Request.AirlineCode, command.Request.FlightNumber,
            command.Request.Sta, command.Request.PeriodBegin, command.Request.PeriodEnd, command.Request.OriginAirportCode),
            command.Request);

        db.Add(flight);
        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleArrivalFlightQueryHandlers.ToResponse(flight);
    }

    internal static SeasonalFlightScheduleArrivalFlight Build(SeasonalFlightScheduleArrivalFlight flight, SeasonalFlightScheduleArrivalFlightRequest r)
    {
        flight.UpdateOrigin(r.OriginAirportCode);
        flight.UpdateCallSign(r.CallSign);
        flight.UpdateStatus(r.ScheduleFlightStatusCode);
        flight.UpdateCountryType(r.CountryType);
        flight.UpdateAircraftType(r.AircraftTypeCode);
        flight.UpdateOperationalDays(r.OpsDays ?? string.Empty, r.Frequency, r.IsOverMidnight);
        flight.UpdateDirection(r.DirectionCode, r.IsTransit);
        flight.UpdateComment(r.Comment);
        flight.ReplaceRoutings(r.Routings.Select(x => (x.AirportCode, x.LegNumber, x.DirectionCode, x.Sta, x.Std)));
        return flight;
    }
}
