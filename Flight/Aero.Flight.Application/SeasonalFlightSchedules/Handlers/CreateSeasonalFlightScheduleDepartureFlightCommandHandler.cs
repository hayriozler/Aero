using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;




public sealed class CreateSeasonalFlightScheduleDepartureFlightCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<CreateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse>
{
    public async Task<SeasonalFlightScheduleDepartureFlightResponse> HandleAsync(CreateSeasonalFlightScheduleDepartureFlightCommand command, CancellationToken ct)
    {
        var flight = Build(SeasonalFlightScheduleDepartureFlight.Create(
            command.Request.ScheduleSeasonCode, command.Request.AirlineCode, command.Request.FlightNumber,
            command.Request.Std, command.Request.PeriodBegin, command.Request.PeriodEnd, command.Request.DestinationAirportCode),
            command.Request);

        db.Add(flight);
        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleDepartureFlightQueryHandlers.ToResponse(flight);
    }

    internal static SeasonalFlightScheduleDepartureFlight Build(SeasonalFlightScheduleDepartureFlight flight, SeasonalFlightScheduleDepartureFlightRequest r)
    {
        flight.UpdateDestination(r.DestinationAirportCode);
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
