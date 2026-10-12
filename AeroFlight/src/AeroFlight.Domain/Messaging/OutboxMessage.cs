namespace AeroFlight.Domain.Messaging;

public sealed class OutboxMessage
{
    public long Id { get; private set; }
    public string Exchange { get; private set; } = string.Empty;
    public string RoutingKey { get; private set; } = string.Empty;
    public string MessageType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }

    private OutboxMessage() { }

    private OutboxMessage(string exchange, string routingKey, string messageType, string payload, DateTimeOffset occurredAt)
    {
        Exchange = exchange;
        RoutingKey = routingKey;
        MessageType = messageType;
        Payload = payload;
        OccurredAt = occurredAt;
    }

    public static OutboxMessage Create(string exchange, string routingKey, string messageType, string payload, DateTimeOffset occurredAt)
        => new(exchange, routingKey, messageType, payload, occurredAt);

    public void MarkPublished(DateTimeOffset at) => PublishedAt = at;

    public void MarkFailed(string error)
    {
        Attempts++;
        LastError = error;
    }
}
