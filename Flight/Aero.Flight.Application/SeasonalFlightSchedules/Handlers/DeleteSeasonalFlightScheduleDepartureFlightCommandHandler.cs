using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class DeleteSeasonalFlightScheduleDepartureFlightCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<DeleteSeasonalFlightScheduleDepartureFlightCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteSeasonalFlightScheduleDepartureFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleDepartureFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        db.Remove(flight);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
