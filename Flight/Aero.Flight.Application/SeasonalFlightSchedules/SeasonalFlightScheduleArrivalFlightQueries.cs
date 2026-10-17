using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules;

public sealed record ListSeasonalFlightScheduleArrivalFlightsQuery(string? ScheduleSeasonCode);
public sealed record GetSeasonalFlightScheduleArrivalFlightQuery(int Id);

public sealed class ListSeasonalFlightScheduleArrivalFlightsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListSeasonalFlightScheduleArrivalFlightsQuery, IReadOnlyList<SeasonalFlightScheduleArrivalFlightResponse>>
{
    public async Task<IReadOnlyList<SeasonalFlightScheduleArrivalFlightResponse>> HandleAsync(ListSeasonalFlightScheduleArrivalFlightsQuery query, CancellationToken ct)
    {
        var items = await db.Query<SeasonalFlightScheduleArrivalFlight>()
            .Include(f => f.Routings)
            .Where(f => query.ScheduleSeasonCode == null || f.ScheduleSeasonCode == query.ScheduleSeasonCode)
            .OrderBy(f => f.Id)
            .ToListAsync(ct);
        return [.. items.Select(SeasonalFlightScheduleArrivalFlightQueryHandlers.ToResponse)];
    }
}

public sealed class GetSeasonalFlightScheduleArrivalFlightQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetSeasonalFlightScheduleArrivalFlightQuery, SeasonalFlightScheduleArrivalFlightResponse?>
{
    public async Task<SeasonalFlightScheduleArrivalFlightResponse?> HandleAsync(GetSeasonalFlightScheduleArrivalFlightQuery query, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleArrivalFlight>()
            .Include(f => f.Routings)
            .FirstOrDefaultAsync(f => f.Id == query.Id, ct);
        return flight is null ? null : SeasonalFlightScheduleArrivalFlightQueryHandlers.ToResponse(flight);
    }
}

internal static class SeasonalFlightScheduleArrivalFlightQueryHandlers
{
    public static SeasonalFlightScheduleArrivalFlightResponse ToResponse(SeasonalFlightScheduleArrivalFlight f) => new(
        f.Id, f.ScheduleSeasonCode, f.AirlineCode, f.FlightNumber, f.CallSign, f.ScheduleFlightStatusCode,
        f.CountryType, f.AircraftTypeCode, f.OpsDays, f.Frequency, f.IsOverMidnight,
        f.DirectionCode, f.IsTransit, f.Comment,
        f.Sta, f.PeriodBegin, f.PeriodEnd, f.OriginAirportCode, f.LinkedDepartureFlightId,
        [.. f.Routings.Select(r => new SeasonalFlightScheduleRoutingResponse(r.Id, r.AirportCode, r.LegNumber, r.DirectionCode, r.Sta, r.Std))]);
}
