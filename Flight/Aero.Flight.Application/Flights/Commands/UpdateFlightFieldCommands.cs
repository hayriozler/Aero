using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.Resources;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Rules;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Commands;

public sealed record UpdateFlightStatusCommand<TFlight>(long Id, string StatusCode) where TFlight : Aero.Flights.Domain.Ops.Flight;
public sealed record UpdateFlightTerminalCommand<TFlight>(long Id, string? TerminalCode) where TFlight : Aero.Flights.Domain.Ops.Flight;
public sealed record UpdateFlightRegistrationCommand<TFlight>(long Id, string? AircraftCode) where TFlight : Aero.Flights.Domain.Ops.Flight;

public sealed class UpdateFlightStatusCommandHandler<TFlight>(IAeroFlightDbContext db, IRuleEngine ruleEngine, IResourceAutoCloseService resourceAutoClose)
    : ICommandHandler<UpdateFlightStatusCommand<TFlight>, bool>
    where TFlight : Aero.Flights.Domain.Ops.Flight
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
    where TFlight : Aero.Flights.Domain.Ops.Flight
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
    where TFlight : Aero.Flights.Domain.Ops.Flight
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
