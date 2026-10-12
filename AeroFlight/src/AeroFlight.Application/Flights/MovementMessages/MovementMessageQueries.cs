using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Ops.MovementMessages;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.MovementMessages;

public sealed record ListMovementMessagesQuery<TMessage>(long FlightId) where TMessage : MovementMessage;

public sealed class ListMovementMessagesQueryHandler<TMessage>(IAeroFlightDbContext db)
    : IQueryHandler<ListMovementMessagesQuery<TMessage>, IReadOnlyList<MovementMessageResponse>>
    where TMessage : MovementMessage
{
    public async Task<IReadOnlyList<MovementMessageResponse>> HandleAsync(ListMovementMessagesQuery<TMessage> query, CancellationToken ct)
    {
        var items = await db.Query<TMessage>()
            .Where(m => m.FlightId == query.FlightId)
            .OrderByDescending(m => m.ReceivedAt)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static MovementMessageResponse ToResponse(TMessage m) => new(m.Id, m.FlightId, m.MessageType, m.Content, m.ReceivedAt);
}
