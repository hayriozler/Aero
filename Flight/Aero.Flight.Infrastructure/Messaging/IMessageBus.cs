namespace Aero.Flights.Infrastructure.Messaging;

public sealed record MessageBusDelivery(string RoutingKey, ReadOnlyMemory<byte> Body, IReadOnlyDictionary<string, object?>? Headers);

public interface IMessagePublisher : IAsyncDisposable
{
    Task DeclareExchangeAsync(string exchange, CancellationToken ct);

    Task PublishAsync(string exchange, string routingKey, ReadOnlyMemory<byte> body,
        IReadOnlyDictionary<string, object?>? headers, CancellationToken ct);
}

// Wraps RabbitMQ.Client so application/infrastructure code never calls IConnection/IChannel directly.
public interface IMessageBus
{
    // One publisher per unit of work (e.g. one Quartz firing) — RabbitMQ channels aren't safe for unbounded concurrent use.
    Task<IMessagePublisher> CreatePublisherAsync(CancellationToken ct);

    // Consumes until ct is cancelled; onMessage returns true to ack, false to nack without requeue.
    Task SubscribeAsync(string queue, string exchange, string routingKeyPattern,
        Func<MessageBusDelivery, CancellationToken, Task<bool>> onMessage, CancellationToken ct);

    // For a queue nothing consumes from directly (e.g. a debug sink inspected via the management UI).
    Task DeclareSinkAsync(string queue, string exchange, string routingKeyPattern, int? maxQueueLength, CancellationToken ct);
}
