using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AeroFlight.Infrastructure.Messaging;

internal sealed class RabbitMqMessageBus(IConnection connection, ILogger<RabbitMqMessageBus> logger) : IMessageBus
{
    public async Task<IMessagePublisher> CreatePublisherAsync(CancellationToken ct)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        return new RabbitMqPublisher(channel);
    }

    public async Task SubscribeAsync(string queue, string exchange, string routingKeyPattern,
        Func<MessageBusDelivery, CancellationToken, Task<bool>> onMessage, CancellationToken ct)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await channel.QueueBindAsync(queue, exchange, routingKeyPattern, cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var delivery = new MessageBusDelivery(ea.RoutingKey, ea.Body, (IReadOnlyDictionary<string, object?>?)ea.BasicProperties?.Headers);
            try
            {
                var acked = await onMessage(delivery, ct);
                if (acked)
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
                else
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle message with routing key {RoutingKey} on queue {Queue}.", ea.RoutingKey, queue);
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
            }
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken: ct);
        ct.Register(channel.Dispose);
    }

    public async Task DeclareSinkAsync(string queue, string exchange, string routingKeyPattern, int? maxQueueLength, CancellationToken ct)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);

        var arguments = maxQueueLength is { } max
            ? new Dictionary<string, object?> { ["x-max-length"] = max }
            : null;
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: arguments, cancellationToken: ct);
        await channel.QueueBindAsync(queue, exchange, routingKeyPattern, cancellationToken: ct);
    }

    private sealed class RabbitMqPublisher(IChannel channel) : IMessagePublisher
    {
        public async Task DeclareExchangeAsync(string exchange, CancellationToken ct) =>
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);

        public async Task PublishAsync(string exchange, string routingKey, ReadOnlyMemory<byte> body,
            IReadOnlyDictionary<string, object?>? headers, CancellationToken ct)
        {
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
                Headers = headers is null ? null : new Dictionary<string, object?>(headers),
            };
            await channel.BasicPublishAsync(exchange, routingKey, mandatory: false, properties, body, cancellationToken: ct);
        }

        public ValueTask DisposeAsync() => channel.DisposeAsync();
    }
}
