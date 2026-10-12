using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.Resources;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Rules;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Commands;

// Lightweight single-field updates for the flights grid's inline editing — a full
// Update{Arrival|Departure}FlightCommand needs ~30 fields the grid row doesn't carry, so these
// exist to avoid a "fetch full detail, patch one field, PUT everything back" round trip.
public sealed record UpdateFlightStatusCommand<TFlight>(long Id, string StatusCode) where TFlight : Flight;
public sealed record UpdateFlightTerminalCommand<TFlight>(long Id, string? TerminalCode) where TFlight : Flight;
public sealed record UpdateFlightRegistrationCommand<TFlight>(long Id, string? AircraftCode) where TFlight : Flight;

public sealed class UpdateFlightStatusCommandHandler<TFlight>(IAeroFlightDbContext db, IRuleEngine ruleEngine, IResourceAutoCloseService resourceAutoClose)
    : ICommandHandler<UpdateFlightStatusCommand<TFlight>, bool>
    where TFlight : Flight
{
    public async Task<bool> HandleAsync(UpdateFlightStatusCommand<TFlight> command, CancellationToken ct)
    {
        var flight = await db.Query<TFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        UpdateFlightSteps.ApplyStatus(flight, command.StatusCode);

        await ruleEngine.EvaluateAsync(flight is ArrivalFlight ? RuleTrigger.ArrivalUpdated : RuleTrigger.DepartureUpdated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightUpdated, flight, ct);
        if (flight is ArrivalFlight arrival)
            await resourceAutoClose.CloseAllIfCompletedAsync(arrival, ct);
        else if (flight is DepartureFlight departure)
            await resourceAutoClose.CloseAllIfCompletedAsync(departure, ct);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

public sealed class UpdateFlightTerminalCommandHandler<TFlight>(IAeroFlightDbContext db, IRuleEngine ruleEngine)
    : ICommandHandler<UpdateFlightTerminalCommand<TFlight>, bool>
    where TFlight : Flight
{
    public async Task<bool> HandleAsync(UpdateFlightTerminalCommand<TFlight> command, CancellationToken ct)
    {
        var flight = await db.Query<TFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        flight.UpdateTerminal(command.TerminalCode);

        await ruleEngine.EvaluateAsync(flight is ArrivalFlight ? RuleTrigger.ArrivalUpdated : RuleTrigger.DepartureUpdated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightUpdated, flight, ct);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

public sealed class UpdateFlightRegistrationCommandHandler<TFlight>(IAeroFlightDbContext db, IRuleEngine ruleEngine, IFlightAutoLinker autoLinker)
    : ICommandHandler<UpdateFlightRegistrationCommand<TFlight>, bool>
    where TFlight : Flight
{
    public async Task<bool> HandleAsync(UpdateFlightRegistrationCommand<TFlight> command, CancellationToken ct)
    {
        var flight = await db.Query<TFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        flight.UpdateAircraft(command.AircraftCode, flight.AircraftTypeCode, flight.OperatingAirlineCode, flight.AircraftOwner);

        await ruleEngine.EvaluateAsync(flight is ArrivalFlight ? RuleTrigger.ArrivalUpdated : RuleTrigger.DepartureUpdated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightUpdated, flight, ct);
        await db.SaveChangesAsync(ct);

        // A registration change may newly match an unlinked flight on the other side.
        if (flight is ArrivalFlight arrival)
            await autoLinker.TryLinkArrivalAsync(arrival, ct);
        else if (flight is DepartureFlight departure)
            await autoLinker.TryLinkDepartureAsync(departure, ct);

        return true;
    }
}
