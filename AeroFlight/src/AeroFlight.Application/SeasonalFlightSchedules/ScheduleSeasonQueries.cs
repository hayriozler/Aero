using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.SeasonalFlightSchedules;
using AeroFlight.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.SeasonalFlightSchedules;

public sealed record ListScheduleSeasonsQuery;
public sealed record GetScheduleSeasonQuery(string Code);

public sealed class ListScheduleSeasonsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListScheduleSeasonsQuery, IReadOnlyList<ScheduleSeasonResponse>>
{
    public async Task<IReadOnlyList<ScheduleSeasonResponse>> HandleAsync(ListScheduleSeasonsQuery query, CancellationToken ct)
    {
        var items = await db.Query<ScheduleSeason>().OrderBy(s => s.Id).ToListAsync(ct);
        return [.. items.Select(ScheduleSeasonQueryHandlers.ToResponse)];
    }
}

public sealed class GetScheduleSeasonQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetScheduleSeasonQuery, ScheduleSeasonResponse?>
{
    public async Task<ScheduleSeasonResponse?> HandleAsync(GetScheduleSeasonQuery query, CancellationToken ct)
    {
        var season = await db.FindAsync<ScheduleSeason>(query.Code, ct);
        return season is null ? null : ScheduleSeasonQueryHandlers.ToResponse(season);
    }
}

internal static class ScheduleSeasonQueryHandlers
{
    public static ScheduleSeasonResponse ToResponse(ScheduleSeason s) => new(s.Id, s.Description, s.BeginDate, s.EndDate);
}
