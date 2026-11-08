using Aero.Flights.Application.SeasonalFlightSchedules.Commands;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Handlers;

public sealed class UpdateSeasonalFlightScheduleArrivalFlightCommandHandler(AeroFlightDbContext db)
    : ICommandHandler<UpdateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse?>
{
    public async Task<SeasonalFlightScheduleArrivalFlightResponse?> HandleAsync(UpdateSeasonalFlightScheduleArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleArrivalFlight>()
            .Include(f => f.Routings)
            .FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return null;

        var r = command.Request;
        flight.UpdateSta(r.Sta);
        flight.UpdatePeriod(r.PeriodBegin, r.PeriodEnd);
        CreateSeasonalFlightScheduleArrivalFlightCommandHandler.Build(flight, r);

        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleArrivalFlightQueryHandlers.ToResponse(flight);
    }
}
