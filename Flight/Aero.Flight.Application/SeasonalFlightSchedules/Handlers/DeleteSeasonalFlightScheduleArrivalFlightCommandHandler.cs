using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class DeleteSeasonalFlightScheduleArrivalFlightCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<DeleteSeasonalFlightScheduleArrivalFlightCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteSeasonalFlightScheduleArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleArrivalFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        db.Remove(flight);
        await db.SaveChangesAsync(ct);
        return true;
    }
}