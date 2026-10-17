using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Rules;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Commands;

public sealed class DeleteArrivalFlightCommandHandler(IAeroFlightDbContext db, IRuleEngine ruleEngine) : ICommandHandler<DeleteArrivalFlightCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<ArrivalFlight>()
            .Include(f => f.Passengers)
            .Include(f => f.OpsBelts)
            .Include(f => f.OpsGates)
            .Include(f => f.Routings)
            .Include(f => f.Services)
            .Include(f => f.CodeShares)
            .Include(f => f.Delays)
            .FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        await DeleteResourcesAsync(flight.Id, isArrival: true, ct);

        if (flight.DepartureFlightId is { } departureFlightId)
        {
            var linked = await db.Query<DepartureFlight>()
                .Include(f => f.Passengers)
                .Include(f => f.OpsBelts)
                .Include(f => f.OpsGates)
                .Include(f => f.OpsCounters)
                .Include(f => f.Routings)
                .Include(f => f.Services)
                .Include(f => f.CodeShares)
                .Include(f => f.Delays)
                .FirstOrDefaultAsync(f => f.Id == departureFlightId, ct);

            if (linked is not null)
            {
                if (command.DeleteLinkedFlight)
                {
                    await DeleteResourcesAsync(linked.Id, isArrival: false, ct);
                    await ruleEngine.EvaluateAsync(RuleTrigger.DepartureDeleted, linked, ct);
                    await ruleEngine.EvaluateAsync(RuleTrigger.FlightDeleted, linked, ct);
                    db.Remove(linked);
                }
                else
                {
                    linked.ClearArrivalLink();
                }
            }
        }

        await ruleEngine.EvaluateAsync(RuleTrigger.ArrivalDeleted, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightDeleted, flight, ct);

        db.Remove(flight);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private async Task DeleteResourcesAsync(long flightId, bool isArrival, CancellationToken ct)
    {
        var parkPositions = isArrival
            ? await db.Query<OpsFlightParkPosition>().Where(s => s.ArrivalFlightId == flightId).ToListAsync(ct)
            : await db.Query<OpsFlightParkPosition>().Where(s => s.DepartureFlightId == flightId).ToListAsync(ct);
        foreach (var parkPosition in parkPositions)
            db.Remove(parkPosition);
    }
}
