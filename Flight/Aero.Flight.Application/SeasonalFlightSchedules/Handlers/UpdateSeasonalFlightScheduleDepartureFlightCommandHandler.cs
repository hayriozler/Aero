using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class UpdateSeasonalFlightScheduleDepartureFlightCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<UpdateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse?>
{
    public async Task<SeasonalFlightScheduleDepartureFlightResponse?> HandleAsync(UpdateSeasonalFlightScheduleDepartureFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleDepartureFlight>()
            .Include(f => f.Routings)
            .FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return null;

        var r = command.Request;
        flight.UpdateStd(r.Std, r.Ptd);
        flight.UpdatePeriod(r.PeriodBegin, r.PeriodEnd);
        CreateSeasonalFlightScheduleDepartureFlightCommandHandler.Build(flight, r);

        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleDepartureFlightQueryHandlers.ToResponse(flight);
    }
}
