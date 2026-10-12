using System.Text.Json;

namespace AeroFlight.Contracts.Outbound;

// Data is the full record for created/updated, null for deleted.
public sealed record MetadataDataChanged(
    string Entity,
    string Operation,
    string Code,
    JsonElement? Data,
    DateTimeOffset OccurredAt);
