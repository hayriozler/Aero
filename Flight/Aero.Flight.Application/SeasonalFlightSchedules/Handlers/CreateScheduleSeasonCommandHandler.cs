using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class CreateScheduleSeasonCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<CreateScheduleSeasonCommand, ScheduleSeasonResponse>
{
    public async Task<ScheduleSeasonResponse> HandleAsync(CreateScheduleSeasonCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var season = ScheduleSeason.Create(r.Code, r.BeginDate, r.EndDate, r.Description);
        db.Add(season);
        await db.SaveChangesAsync(ct);
        return ScheduleSeasonQueryHandlers.ToResponse(season);
    }
}
