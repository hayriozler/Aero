using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using AeroFlight.Contracts.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules;

public sealed record ListSeasonalFlightScheduleDepartureFlightsQuery(string? ScheduleSeasonCode);
public sealed record GetSeasonalFlightScheduleDepartureFlightQuery(int Id);

public sealed class ListSeasonalFlightScheduleDepartureFlightsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListSeasonalFlightScheduleDepartureFlightsQuery, IReadOnlyList<SeasonalFlightScheduleDepartureFlightResponse>>
{
    public async Task<IReadOnlyList<SeasonalFlightScheduleDepartureFlightResponse>> HandleAsync(ListSeasonalFlightScheduleDepartureFlightsQuery query, CancellationToken ct)
    {
        var items = await db.Query<SeasonalFlightScheduleDepartureFlight>()
            .Include(f => f.Routings)
            .Where(f => query.ScheduleSeasonCode == null || f.ScheduleSeasonCode == query.ScheduleSeasonCode)
            .OrderBy(f => f.Id)
            .ToListAsync(ct);
        return [.. items.Select(SeasonalFlightScheduleDepartureFlightQueryHandlers.ToResponse)];
    }
}

public sealed class GetSeasonalFlightScheduleDepartureFlightQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetSeasonalFlightScheduleDepartureFlightQuery, SeasonalFlightScheduleDepartureFlightResponse?>
{
    public async Task<SeasonalFlightScheduleDepartureFlightResponse?> HandleAsync(GetSeasonalFlightScheduleDepartureFlightQuery query, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleDepartureFlight>()
            .Include(f => f.Routings)
            .FirstOrDefaultAsync(f => f.Id == query.Id, ct);
        return flight is null ? null : SeasonalFlightScheduleDepartureFlightQueryHandlers.ToResponse(flight);
    }
}

internal static class SeasonalFlightScheduleDepartureFlightQueryHandlers
{
    public static SeasonalFlightScheduleDepartureFlightResponse ToResponse(SeasonalFlightScheduleDepartureFlight f) => new(
        f.Id, f.ScheduleSeasonCode, f.AirlineCode, f.FlightNumber, f.CallSign, f.ScheduleFlightStatusCode,
        f.CountryType, f.AircraftTypeCode, f.OpsDays, f.Frequency, f.IsOverMidnight,
        f.DirectionCode, f.IsTransit, f.Comment,
        f.Std, f.Ptd, f.PeriodBegin, f.PeriodEnd, f.DestinationAirportCode, f.LinkedArrivalFlightId,
        [.. f.Routings.Select(r => new SeasonalFlightScheduleRoutingResponse(r.Id, r.AirportCode, r.LegNumber, r.DirectionCode, r.Sta, r.Std))]);
}
