using Aero.Fids.Infrastructure.Flights;

namespace Aero.Fids.Web.Boards;

// Compiled once per tenant from that tenant's StatusMapping rows — mirrors FlightFieldMapper.
// Anything not found in the table (or a blank source value) maps to Unknown rather than throwing —
// a missing mapping is a config gap, not a reason to drop the flight/resource off the board.
public sealed class StatusMapper
{
    private readonly IReadOnlyDictionary<string, string> _flightDefaults;
    private readonly IReadOnlyDictionary<(string FlightType, string External), string> _flightTyped;
    private readonly IReadOnlyDictionary<(string Dimension, string External), string> _resource;

    private StatusMapper(
        IReadOnlyDictionary<string, string> flightDefaults,
        IReadOnlyDictionary<(string, string), string> flightTyped,
        IReadOnlyDictionary<(string, string), string> resource)
    {
        _flightDefaults = flightDefaults;
        _flightTyped = flightTyped;
        _resource = resource;
    }

    public static StatusMapper Build(IEnumerable<StatusMapping> mappings)
    {
        var flightDefaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var flightTyped = new Dictionary<(string, string), string>();
        var resource = new Dictionary<(string, string), string>();

        foreach (var mapping in mappings)
        {
            var externalKey = mapping.ExternalValue.ToUpperInvariant();

            if (mapping.Dimension == StatusMappingDimensions.Flight)
            {
                if (mapping.FlightType is null)
                    flightDefaults[externalKey] = mapping.InternalValue;
                else
                    flightTyped[(mapping.FlightType, externalKey)] = mapping.InternalValue;
            }
            else
            {
                resource[(mapping.Dimension, externalKey)] = mapping.InternalValue;
            }
        }

        return new StatusMapper(flightDefaults, flightTyped, resource);
    }

    public string MapFlightStatus(string? externalValue, string flightType)
    {
        if (string.IsNullOrWhiteSpace(externalValue))
            return InternalFlightStatuses.Unknown;

        var key = externalValue.ToUpperInvariant();
        if (_flightTyped.TryGetValue((flightType, key), out var typed))
            return typed;

        return _flightDefaults.GetValueOrDefault(key, InternalFlightStatuses.Unknown);
    }

    // dimension is one of StatusMappingDimensions.Gate/Belt/Counter.
    public string MapResourceStatus(string? externalValue, string dimension)
    {
        if (string.IsNullOrWhiteSpace(externalValue))
            return InternalResourceStatuses.Unknown;

        return _resource.GetValueOrDefault((dimension, externalValue.ToUpperInvariant()), InternalResourceStatuses.Unknown);
    }
}
