using AeroFids.Infrastructure.Boards;
using AeroFids.Infrastructure.Flights;
using AeroFids.Service.Boards;
using AeroFlight.Contracts.Fids;
using AeroFlight.Contracts.Topology;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AeroFids.Service.Messaging;

internal sealed class FlightUpdatesConsumer(
    IConnection connection, IFlightBoardStore store, IFieldMappingRegistry fieldMappings,
    IStatusMappingRegistry statusMappings,
    IHubContext<BoardHub> hub, ILogger<FlightUpdatesConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await channel.ExchangeDeclareAsync(ExchangeNames.Flights, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QueueNames.AeroFidsFlightUpdates, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(QueueNames.AeroFidsFlightUpdates, ExchangeNames.Flights, routingKey: "#", cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await ApplyAsync(ea, stoppingToken);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process flight update with routing key {RoutingKey}.", ea.RoutingKey);
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(QueueNames.AeroFidsFlightUpdates, autoAck: false, consumer, cancellationToken: stoppingToken);
        stoppingToken.Register(channel.Dispose);
    }

    private async Task ApplyAsync(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        var tenantId = ReadTenantHeader(ea.BasicProperties);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            logger.LogWarning("Dropping flight update with no 'tenant' header; routing key {RoutingKey}.", ea.RoutingKey);
            return;
        }

        using var document = JsonDocument.Parse(ea.Body);
        var mapper = await fieldMappings.GetMapperAsync(tenantId, ct);
        var statusMapper = await statusMappings.GetMapperAsync(tenantId, ct);
        var entry = mapper.Map(document.RootElement, statusMapper);
        if (entry is null)
        {
            logger.LogWarning(
                "Could not map flight update for tenant {TenantId} to a FidsFlight — check its field mapping; routing key {RoutingKey}.",
                tenantId, ea.RoutingKey);
            return;
        }

        store.TryGetExisting(tenantId, entry.FlightType, entry.FlightId, out var previous);
        store.Apply(tenantId, entry);

        await PushBoardAsync(tenantId, entry, ct);
        await PushResourcesAsync(tenantId, mapper, previous, entry, ct);
    }

    private async Task PushBoardAsync(string tenantId, FidsFlight entry, CancellationToken ct)
    {
        var flightListType = entry.FlightType == FlightTypes.Arrival
            ? DisplayClientTypes.ArrivalFlightList
            : DisplayClientTypes.DepartureFlightList;

        await hub.Clients.Group(BoardGroups.NameForList(tenantId, flightListType)).SendAsync("flightUpdated", entry, ct);
        await hub.Clients.Group(BoardGroups.NameForList(tenantId, DisplayClientTypes.FullFlightList)).SendAsync("flightUpdated", entry, ct);
    }

    private async Task PushResourcesAsync(string tenantId, FlightFieldMapper mapper, FidsFlight? previous, FidsFlight entry, CancellationToken ct)
    {
        await PushResourceKindAsync(tenantId, mapper, previous, entry, ResourceKinds.Gate, ct);
        await PushResourceKindAsync(tenantId, mapper, previous, entry, ResourceKinds.Belt, ct);
        await PushResourceKindAsync(tenantId, mapper, previous, entry, ResourceKinds.Spot, ct);
        if (entry.FlightType == FlightTypes.Departure)
            await PushResourceKindAsync(tenantId, mapper, previous, entry, ResourceKinds.Counter, ct);
    }
    private async Task PushResourceKindAsync(
        string tenantId, FlightFieldMapper mapper, FidsFlight? previous, FidsFlight entry, string kind, CancellationToken ct)
    {
        var codes = entry.Resources.Concat(previous?.Resources ?? [])
            .Where(r => string.Equals(r.Kind, kind, StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Code)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (codes.Count == 0)
            return;

        bool IsShown(string? externalStatus) => mapper.IsResourceShown(kind, externalStatus);
        var displayType = DisplayTypeFor(kind, entry.FlightType);

        foreach (var code in codes)
        {
            var previousItem = ResourceDatasetProjector.ProjectOne(previous, kind, code, IsShown);
            var currentItem = ResourceDatasetProjector.ProjectOne(entry, kind, code, IsShown);
            if (Equals(previousItem, currentItem))
                continue;

            var payload = currentItem ?? new ResourceUnassigned(entry.FlightId, code);
            await hub.Clients.Group(BoardGroups.NameForResource(tenantId, displayType, code))
                .SendAsync(ResourceDatasetProjector.EventNameFor(kind), payload, ct);
        }

        var overviewFlights = kind switch
        {
            ResourceKinds.Counter => store.GetBoard(tenantId, FlightTypes.Departure),
            ResourceKinds.Belt => store.GetBoard(tenantId, entry.FlightType),
            _ => store.GetAll(tenantId),
        };
        var overview = ResourceDatasetProjector.ProjectOverview(kind, overviewFlights, IsShown);
        await hub.Clients.Group(BoardGroups.NameForResourceOverview(tenantId, displayType)).SendAsync("boardSnapshot", overview, ct);
    }

    // ArrivalBelt/DepartureBelt are two DisplayClientTypes sharing one ResourceKinds.Belt — group
    // names key on the DisplayClientType (what a client actually joins), not the kind.
    private static string DisplayTypeFor(string kind, string flightType) => kind switch
    {
        ResourceKinds.Belt => flightType == FlightTypes.Arrival ? DisplayClientTypes.ArrivalBelt : DisplayClientTypes.DepartureBelt,
        _ => kind,
    };

    private static string? ReadTenantHeader(IReadOnlyBasicProperties? properties)
    {
        if (properties?.Headers is { } headers && headers.TryGetValue(MessageHeaders.Tenant, out var raw))
        {
            return raw switch
            {
                byte[] bytes => Encoding.UTF8.GetString(bytes),
                string text => text,
                _ => raw?.ToString(),
            };
        }

        return null;
    }
}
