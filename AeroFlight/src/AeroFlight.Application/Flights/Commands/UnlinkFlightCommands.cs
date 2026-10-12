using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Ops;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Commands;

public sealed record UnlinkArrivalFlightCommand(long Id);

public sealed record UnlinkDepartureFlightCommand(long Id);

public sealed class UnlinkArrivalFlightCommandHandler(IAeroFlightDbContext db) : ICommandHandler<UnlinkArrivalFlightCommand, bool>
{
    public async Task<bool> HandleAsync(UnlinkArrivalFlightCommand command, CancellationToken ct)
    {
        var arrival = await db.Query<ArrivalFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (arrival is null || arrival.DepartureFlightId is not { } departureFlightId)
            return false;

        var departure = await db.Query<DepartureFlight>().FirstOrDefaultAsync(f => f.Id == departureFlightId, ct);
        departure?.ClearArrivalLink();
        arrival.ClearDepartureLink();

        await db.SaveChangesAsync(ct);
        return true;
    }
}

public sealed class UnlinkDepartureFlightCommandHandler(IAeroFlightDbContext db) : ICommandHandler<UnlinkDepartureFlightCommand, bool>
{
    public async Task<bool> HandleAsync(UnlinkDepartureFlightCommand command, CancellationToken ct)
    {
        var departure = await db.Query<DepartureFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (departure is null || departure.ArrivalFlightId is not { } arrivalFlightId)
            return false;

        var arrival = await db.Query<ArrivalFlight>().FirstOrDefaultAsync(f => f.Id == arrivalFlightId, ct);
        arrival?.ClearDepartureLink();
        departure.ClearArrivalLink();

        await db.SaveChangesAsync(ct);
        return true;
    }
}
