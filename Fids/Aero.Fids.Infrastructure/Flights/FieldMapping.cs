namespace Aero.Fids.Infrastructure.Flights;

// The full set of fields FidsFlight can be populated from. A tenant maps each one to wherever
// that value actually lives in their source system's message — see FieldMapping below.
public static class FidsFlightFields
{
    public const string FlightId = "FlightId";
    public const string FlightType = "FlightType";

    public const string DesignatorAirlineCodes = "DesignatorAirlineCodes";
    public const string DesignatorFlightNumbers = "DesignatorFlightNumbers";

    public const string FlightDate = "FlightDate";
    public const string Status = "Status";
    public const string ScheduledTime = "ScheduledTime";
    public const string EstimatedTime = "EstimatedTime";
    public const string TerminalCode = "TerminalCode";
    public const string RouteAirportCode = "RouteAirportCode";

    public const string TenantTimeZone = "TenantTimeZone";

    public const string Gates = "Gates";
    public const string GateStatuses = "GateStatuses";
    public const string Belts = "Belts";
    public const string BeltStatuses = "BeltStatuses";
    public const string Counters = "Counters";
    public const string CounterStatuses = "CounterStatuses";
    public const string CounterDisplayCodes = "CounterDisplayCodes";
    public const string Spots = "Spots";
    public const string SpotStatuses = "SpotStatuses";

    public static readonly IReadOnlyList<string> All =
    [
        FlightId, FlightType, DesignatorAirlineCodes, DesignatorFlightNumbers, FlightDate, Status,
        ScheduledTime, EstimatedTime, TerminalCode, RouteAirportCode, TenantTimeZone,
        Gates, GateStatuses, Belts, BeltStatuses, Counters, CounterStatuses, CounterDisplayCodes, Spots, SpotStatuses,
    ];

    public static readonly IReadOnlyList<string> Required = [FlightId, FlightType];
}

public static class FieldMappingRuleOperators
{
    public const string Eq = "Eq";
    public const string StartsWith = "StartsWith";
    public const string In = "In";

    public static readonly IReadOnlyList<string> All = [Eq, StartsWith, In];

    public static bool Matches(string? ruleOperator, string? ruleValue, string? candidate)
    {
        if (ruleOperator is null || ruleValue is null)
            return true;
        if (candidate is null)
            return false;

        return ruleOperator switch
        {
            Eq => string.Equals(candidate, ruleValue, StringComparison.OrdinalIgnoreCase),
            StartsWith => candidate.StartsWith(ruleValue, StringComparison.OrdinalIgnoreCase),
            In => ruleValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Any(v => string.Equals(v, candidate, StringComparison.OrdinalIgnoreCase)),
            _ => true,
        };
    }
}

public sealed class FieldMapping
{
    public long Id { get; private set; }
    public string TargetField { get; private set; } = string.Empty;

    public string? FlightType { get; private set; }
    public string SourcePath { get; private set; } = string.Empty;

    public string? RuleOperator { get; private set; }
    public string? RuleValue { get; private set; }

    private FieldMapping() { }

    public static FieldMapping Create(string targetField, string? flightType, string sourcePath) => new()
    {
        TargetField = targetField,
        FlightType = flightType,
        SourcePath = sourcePath,
    };

    public void Update(string sourcePath) => SourcePath = sourcePath;

    public void SetRule(string? ruleOperator, string? ruleValue)
    {
        RuleOperator = ruleOperator;
        RuleValue = ruleValue;
    }
}
