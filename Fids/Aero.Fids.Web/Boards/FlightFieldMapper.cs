using Aero.Fids.Infrastructure.Flights;
using System.Text.Json;

namespace Aero.Fids.Web.Boards;

public sealed class FlightFieldMapper
{
    private readonly IReadOnlyDictionary<string, string[]> _defaults;
    private readonly IReadOnlyDictionary<(string Target, string FlightType), string[]> _typed;
    private readonly IReadOnlyDictionary<string, (string Operator, string Value)> _rules;

    private FlightFieldMapper(
        IReadOnlyDictionary<string, string[]> defaults, IReadOnlyDictionary<(string, string), string[]> typed,
        IReadOnlyDictionary<string, (string, string)> rules)
    {
        _defaults = defaults;
        _typed = typed;
        _rules = rules;
    }

    public static FlightFieldMapper Build(IEnumerable<FieldMapping> mappings)
    {
        var defaults = new Dictionary<string, string[]>();
        var typed = new Dictionary<(string, string), string[]>();
        var rules = new Dictionary<string, (string, string)>();

        foreach (var mapping in mappings)
        {
            var segments = mapping.SourcePath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (mapping.FlightType is null)
                defaults[mapping.TargetField] = segments;
            else
                typed[(mapping.TargetField, mapping.FlightType)] = segments;

            if (mapping.RuleOperator is not null && mapping.RuleValue is not null)
                rules[mapping.TargetField] = (mapping.RuleOperator, mapping.RuleValue);
        }

        return new FlightFieldMapper(defaults, typed, rules);
    }

    public bool IsResourceShown(string kind, string? externalStatus)
    {
        var target = StatusTargetFor(kind);
        return !_rules.TryGetValue(target, out var rule) || FieldMappingRuleOperators.Matches(rule.Operator, rule.Value, externalStatus);
    }

    private static string StatusTargetFor(string kind) => kind switch
    {
        ResourceKinds.Gate => FidsFlightFields.GateStatuses,
        ResourceKinds.Belt => FidsFlightFields.BeltStatuses,
        ResourceKinds.Counter => FidsFlightFields.CounterStatuses,
        ResourceKinds.Spot => FidsFlightFields.SpotStatuses,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown resource kind."),
    };

    public FidsFlight? Map(JsonElement root, StatusMapper statusMapper)
    {
        var flightType = GetString(root, FidsFlightFields.FlightType, null);
        var flightId = GetLong(root, FidsFlightFields.FlightId, flightType);
        if (flightId is null || string.IsNullOrEmpty(flightType))
            return null;

        var tenantTimeZone = GetString(root, FidsFlightFields.TenantTimeZone, flightType) ?? "UTC";
        var scheduledTime = GetDateTimeOffset(root, FidsFlightFields.ScheduledTime, flightType) ?? DateTimeOffset.UtcNow;
        var estimatedTime = GetDateTimeOffset(root, FidsFlightFields.EstimatedTime, flightType);
        var status = statusMapper.MapFlightStatus(GetString(root, FidsFlightFields.Status, flightType), flightType);

        return new FidsFlight(
            flightId.Value,
            flightType,
            BuildDesignators(root, flightType),
            GetDateOnly(root, FidsFlightFields.FlightDate, flightType) ?? DateOnly.FromDateTime(DateTime.UtcNow),
            status,
            scheduledTime,
            TenantLocalTime.Format(scheduledTime, tenantTimeZone),
            estimatedTime,
            TenantLocalTime.FormatOrNull(estimatedTime, tenantTimeZone),
            GetString(root, FidsFlightFields.TerminalCode, flightType),
            GetString(root, FidsFlightFields.RouteAirportCode, flightType) ?? "",
            BuildResources(root, flightType, statusMapper));
    }

    private IReadOnlyList<FidsFlightDesignator> BuildDesignators(JsonElement root, string? flightType)
    {
        var airlineCodes = GetMultiValue(root, FidsFlightFields.DesignatorAirlineCodes, flightType);
        var flightNumbers = GetMultiValue(root, FidsFlightFields.DesignatorFlightNumbers, flightType);

        var count = Math.Min(airlineCodes.Count, flightNumbers.Count);
        var designators = new List<FidsFlightDesignator>(count);
        for (var i = 0; i < count; i++)
            designators.Add(new FidsFlightDesignator(airlineCodes[i], flightNumbers[i]));

        return designators;
    }

    private IReadOnlyList<FidsResource> BuildResources(JsonElement root, string? flightType, StatusMapper statusMapper)
    {
        var resources = new List<FidsResource>();
        AddResources(resources, ResourceKinds.Gate,
            GetMultiValue(root, FidsFlightFields.Gates, flightType), GetMultiValue(root, FidsFlightFields.GateStatuses, flightType), statusMapper);
        AddResources(resources, ResourceKinds.Belt,
            GetMultiValue(root, FidsFlightFields.Belts, flightType), GetMultiValue(root, FidsFlightFields.BeltStatuses, flightType), statusMapper);
        AddResources(resources, ResourceKinds.Counter,
            GetMultiValue(root, FidsFlightFields.Counters, flightType), GetMultiValue(root, FidsFlightFields.CounterStatuses, flightType), statusMapper,
            GetMultiValue(root, FidsFlightFields.CounterDisplayCodes, flightType));
        AddResources(resources, ResourceKinds.Spot,
            GetMultiValue(root, FidsFlightFields.Spots, flightType), GetMultiValue(root, FidsFlightFields.SpotStatuses, flightType), statusMapper);
        return resources;
    }

    private static void AddResources(
        List<FidsResource> resources, string kind, IReadOnlyList<string> codes, IReadOnlyList<string> statuses, StatusMapper statusMapper,
        IReadOnlyList<string>? displayCodes = null)
    {
        for (var i = 0; i < codes.Count; i++)
        {
            var externalStatus = i < statuses.Count ? statuses[i] : null;
            var status = externalStatus is not null ? statusMapper.MapResourceStatus(externalStatus, kind) : null;
            var displayCode = displayCodes is not null && i < displayCodes.Count ? displayCodes[i] : null;
            resources.Add(new FidsResource(kind, codes[i], status, externalStatus, displayCode));
        }
    }

    private string[]? SegmentsFor(string targetField, string? flightType)
    {
        string[]? segments = null;
        if (flightType is not null)
            _typed.TryGetValue((targetField, flightType), out segments);
        return segments ?? _defaults.GetValueOrDefault(targetField);
    }

    private JsonElement? Resolve(JsonElement root, string targetField, string? flightType)
    {
        var segments = SegmentsFor(targetField, flightType);
        if (segments is null)
            return null;

        var current = root;
        foreach (var segment in segments)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(segment, out current))
                return null;
        }

        return current;
    }

    private string? GetString(JsonElement root, string targetField, string? flightType) =>
        Resolve(root, targetField, flightType) is { } e ? JsonElementReaders.GetString(e) : null;

    private long? GetLong(JsonElement root, string targetField, string? flightType) =>
        Resolve(root, targetField, flightType) is { } e ? JsonElementReaders.GetLong(e) : null;

    private DateTimeOffset? GetDateTimeOffset(JsonElement root, string targetField, string? flightType) =>
        Resolve(root, targetField, flightType) is { } e ? JsonElementReaders.GetDateTimeOffset(e) : null;

    private DateOnly? GetDateOnly(JsonElement root, string targetField, string? flightType) =>
        Resolve(root, targetField, flightType) is { } e ? JsonElementReaders.GetDateOnly(e) : null;

    private IReadOnlyList<string> GetMultiValue(JsonElement root, string targetField, string? flightType)
    {
        var segments = SegmentsFor(targetField, flightType);
        if (segments is null)
            return [];

        var arrayIndex = Array.FindIndex(segments, s => s.EndsWith("[]", StringComparison.Ordinal));
        if (arrayIndex < 0)
            return Resolve(root, targetField, flightType) is { } e ? JsonElementReaders.GetMultiValue(e) : [];

        var current = root;
        for (var i = 0; i < arrayIndex; i++)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(segments[i], out current))
                return [];
        }

        var arrayProperty = segments[arrayIndex][..^2];
        if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(arrayProperty, out current) || current.ValueKind != JsonValueKind.Array)
            return [];

        var remaining = segments[(arrayIndex + 1)..];
        var values = new List<string>();
        foreach (var element in current.EnumerateArray())
        {
            var value = element;
            var resolved = true;
            foreach (var segment in remaining)
            {
                if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(segment, out value))
                {
                    resolved = false;
                    break;
                }
            }

            if (resolved && JsonElementReaders.GetString(value) is { } s)
                values.Add(s);
        }

        return values;
    }
}
