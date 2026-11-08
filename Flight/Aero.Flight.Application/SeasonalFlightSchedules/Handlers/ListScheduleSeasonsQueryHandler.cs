using Aero.Flights.Application.SeasonalFlightSchedules.Queries;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class ListScheduleSeasonsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListScheduleSeasonsQuery, IReadOnlyList<ScheduleSeasonResponse>>
{
    public async Task<IReadOnlyList<ScheduleSeasonResponse>> HandleAsync(ListScheduleSeasonsQuery query, CancellationToken ct)
    {
        var items = await db.Query<ScheduleSeason>().OrderBy(s => s.Id).ToListAsync(ct);
        return [.. items.Select(new(s.Id, s.Description, s.BeginDate, s.EndDate))];
    }
}