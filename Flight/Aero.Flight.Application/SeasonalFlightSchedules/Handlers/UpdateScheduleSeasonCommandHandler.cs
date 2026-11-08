using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class UpdateScheduleSeasonCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<UpdateScheduleSeasonCommand, ScheduleSeasonResponse?>
{
    public async Task<ScheduleSeasonResponse?> HandleAsync(UpdateScheduleSeasonCommand command, CancellationToken ct)
    {
        var season = await db.FindAsync<ScheduleSeason>(command.Code, ct);
        if (season is null)
            return null;

        var r = command.Request;
        season.UpdateDates(r.BeginDate, r.EndDate);
        if (r.Description is not null)
            season.UpdateDescription(r.Description);
        await db.SaveChangesAsync(ct);
        return ScheduleSeasonQueryHandlers.ToResponse(season);
    }
}