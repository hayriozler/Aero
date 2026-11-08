using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class DeleteScheduleSeasonCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<DeleteScheduleSeasonCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteScheduleSeasonCommand command, CancellationToken ct)
    {
        var season = await db.FindAsync<ScheduleSeason>(command.Code, ct);
        if (season is null)
            return false;

        var hasFlights = await db.Query<SeasonalFlightScheduleArrivalFlight>().AnyAsync(f => f.ScheduleSeasonCode == command.Code, ct)
            || await db.Query<SeasonalFlightScheduleDepartureFlight>().AnyAsync(f => f.ScheduleSeasonCode == command.Code, ct);
        if (hasFlights)
            throw new InvalidOperationException($"Season '{command.Code}' has schedule flights and cannot be deleted.");

        db.Remove(season);
        await db.SaveChangesAsync(ct);
        return true;
    }
}