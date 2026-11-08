using Aero.Flights.Application.SeasonalFlightSchedules.Queries;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class GetScheduleSeasonQueryHandler(AeroFlightDbContext db)
    : IQueryHandler<GetScheduleSeasonQuery, ScheduleSeasonResponse?>
{
    public async Task<ScheduleSeasonResponse?> HandleAsync(GetScheduleSeasonQuery query, CancellationToken ct)
    {
        var season = await db.FindAsync<ScheduleSeason>(query.Code, ct);
        return season is null ? null : new ScheduleSeasonResponse(season.Id, season.Description, season.BeginDate, season.EndDate);
    }
}
