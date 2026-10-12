using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Delays;

public sealed record ListDelaysQuery<TDelay>(long FlightId) where TDelay : Delay;
public sealed record AddDelayCommand<TDelay>(long FlightId, DelayRequest Request) where TDelay : Delay;
public sealed record UpdateDelayCommand<TDelay>(long FlightId, long DelayId, DelayRequest Request) where TDelay : Delay;
public sealed record DeleteDelayCommand<TDelay>(long FlightId, long DelayId) where TDelay : Delay;

public sealed class ListDelaysQueryHandler<TDelay>(IAeroFlightDbContext db)
    : IQueryHandler<ListDelaysQuery<TDelay>, IReadOnlyList<DelayResponse>>
    where TDelay : Delay
{
    public async Task<IReadOnlyList<DelayResponse>> HandleAsync(ListDelaysQuery<TDelay> query, CancellationToken ct)
    {
        var items = await db.Query<TDelay>()
            .Where(d => d.FlightId == query.FlightId)
            .OrderBy(d => d.Id)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static DelayResponse ToResponse(TDelay d) => new(d.Id, d.FlightId, d.DelayCode, d.DelayMinutes, d.Comment);
}

public sealed class AddDelayCommandHandler<TDelay>(IAeroFlightDbContext db, IDelayFactory<TDelay> factory)
    : ICommandHandler<AddDelayCommand<TDelay>, DelayResponse>
    where TDelay : Delay
{
    public async Task<DelayResponse> HandleAsync(AddDelayCommand<TDelay> command, CancellationToken ct)
    {
        var r = command.Request;
        var delay = factory.Create(command.FlightId, r.DelayCode, r.DelayMinutes, r.Comment);
        db.Add(delay);
        await db.SaveChangesAsync(ct);
        return ListDelaysQueryHandler<TDelay>.ToResponse(delay);
    }
}

public sealed class UpdateDelayCommandHandler<TDelay>(IAeroFlightDbContext db)
    : ICommandHandler<UpdateDelayCommand<TDelay>, DelayResponse?>
    where TDelay : Delay
{
    public async Task<DelayResponse?> HandleAsync(UpdateDelayCommand<TDelay> command, CancellationToken ct)
    {
        var delay = await db.Query<TDelay>().FirstOrDefaultAsync(d => d.Id == command.DelayId && d.FlightId == command.FlightId, ct);
        if (delay is null)
            return null;

        var r = command.Request;
        delay.Update(r.DelayCode, r.DelayMinutes, r.Comment);
        await db.SaveChangesAsync(ct);
        return ListDelaysQueryHandler<TDelay>.ToResponse(delay);
    }
}

public sealed class DeleteDelayCommandHandler<TDelay>(IAeroFlightDbContext db)
    : ICommandHandler<DeleteDelayCommand<TDelay>, bool>
    where TDelay : Delay
{
    public async Task<bool> HandleAsync(DeleteDelayCommand<TDelay> command, CancellationToken ct)
    {
        var delay = await db.Query<TDelay>().FirstOrDefaultAsync(d => d.Id == command.DelayId && d.FlightId == command.FlightId, ct);
        if (delay is null)
            return false;

        db.Remove(delay);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
