using System.Text.Json;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Outbound;
using Aero.Flights.Contracts.Topology;

namespace Aero.Flights.Application.Metadatas;

public sealed class MetadataDataInboundProcessor(IEnumerable<IMetadataInboundApplier> appliers)
    : IInboundMessageProcessor
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    private readonly Dictionary<string, IMetadataInboundApplier> _appliers =
        appliers.ToDictionary(a => a.Entity, StringComparer.OrdinalIgnoreCase);

    public string MessageType => RoutingKeys.Inbound.MetadataDataChanged;

    public async Task ProcessAsync(string payload, CancellationToken ct)
    {
        var message = JsonSerializer.Deserialize<MetadataDataChanged>(payload, _jsonOptions);
        if (message is null || !_appliers.TryGetValue(message.Entity, out var applier))
            return;

        switch (message.Operation)
        {
            case RoutingKeys.MetadataData.Created or RoutingKeys.MetadataData.Updated:
                await applier.UpsertAsync(message.Code, message.Data, ct);
                break;
            case RoutingKeys.MetadataData.Deleted:
                await applier.DeleteAsync(message.Code, ct);
                break;
        }
    }
}
