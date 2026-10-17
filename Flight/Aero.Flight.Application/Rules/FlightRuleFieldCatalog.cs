using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Ops.Services;
using Aero.Flights.Domain.Rules;

namespace Aero.Flights.Application.Rules;

// Tenant-configured defaults for auto-created resource placeholders, sourced from AeroConfiguration
// rows under the "Resources." prefix (see AeroConfigSeeder) — e.g. Resources.Counter.DefaultOffsetMinutes,
// Resources.Gate.DefaultDurationMinutes. Fetched once per RuleEngine.EvaluateAsync call and passed
// through to RuleEvaluator/FlightRuleFieldCatalog rather than read here, so this catalog and
// RuleEvaluator stay I/O-free and unit-testable.
public sealed class ResourceDefaults(IReadOnlyDictionary<string, string> values, IReadOnlyDictionary<string, string?> defaultStatusCodesByKind)
{
    public int DurationMinutes(string kind) =>
        values.TryGetValue($"{kind}.DefaultDurationMinutes", out var v) && int.TryParse(v, out var minutes)
            ? minutes : ResourceDefaultsConstants.FallbackDurationMinutes;

    public int OffsetMinutes(string kind) =>
        values.TryGetValue($"{kind}.DefaultOffsetMinutes", out var v) && int.TryParse(v, out var minutes) ? minutes : 0;

    public string? StatusCode(string kind) => defaultStatusCodesByKind.GetValueOrDefault(kind);
}

public sealed record FlightRuleField(
    string Key,
    string Label,
    RuleFieldDataType DataType,
    IReadOnlyList<RuleTrigger> Triggers,
    bool UsableInCondition,
    bool UsableInAction,
    Func<Flight, object?> Getter,
    // (flight, value, matchValue, defaults) — matchValue is only ever non-null when
    // SupportsMatchValue is true for this field; every other setter ignores it.
    Action<Flight, string?, string?, ResourceDefaults>? Setter,
    IReadOnlyList<RuleActionType>? SupportedActionTypes = null,
    bool SupportsMatchValue = false,
    string? MatchValueLabel = null);

public sealed class FlightRuleFieldCatalog
{
    private static readonly RuleTrigger[] _both =
        [RuleTrigger.FlightCreated, RuleTrigger.FlightUpdated, RuleTrigger.FlightDeleted,
         RuleTrigger.DepartureCreated, RuleTrigger.DepartureDeleted, RuleTrigger.DepartureUpdated,
         RuleTrigger.ArrivalCreated, RuleTrigger.ArrivalUpdated, RuleTrigger.ArrivalDeleted];
    private static readonly RuleTrigger[] _departureOnly = [RuleTrigger.DepartureCreated, RuleTrigger.DepartureDeleted, RuleTrigger.DepartureUpdated];
    private static readonly RuleTrigger[] _arrivalOnly = [RuleTrigger.ArrivalCreated, RuleTrigger.ArrivalUpdated, RuleTrigger.ArrivalDeleted];

    // Resource-count actions are idempotent (RequireCount tops up to the target count instead of
    // always adding N more — see RequireCount), so Updated triggers are safe: re-matching on every
    // save won't pile up duplicates.
    private static readonly RuleTrigger[] _bothCountable =
        [RuleTrigger.FlightCreated, RuleTrigger.FlightUpdated,
         RuleTrigger.DepartureCreated, RuleTrigger.DepartureUpdated,
         RuleTrigger.ArrivalCreated, RuleTrigger.ArrivalUpdated];
    private static readonly RuleTrigger[] _departureCountable =
        [RuleTrigger.FlightCreated, RuleTrigger.FlightUpdated, RuleTrigger.DepartureCreated, RuleTrigger.DepartureUpdated];


    public FlightRuleFieldCatalog() => Fields = Build();

    public IReadOnlyList<FlightRuleField> Fields { get; }

    public bool TryGet(string key, out FlightRuleField field) => Fields.ToDictionary(f => f.Key, StringComparer.OrdinalIgnoreCase).TryGetValue(key, out field!);

    public FlightRuleField? Find(string key) => Fields.ToDictionary(f => f.Key, StringComparer.OrdinalIgnoreCase).TryGetValue(key, out var f) ? f : null;

    private static List<FlightRuleField> Build() =>
    [
        // ── Condition-readable fields ──
        Condition("AirlineCode", "Airline Code", RuleFieldDataType.String, _both, f => f.AirlineCode),
        Condition("FlightNumber", "Flight Number", RuleFieldDataType.String, _both, f => f.FlightNumber),
        Condition("DestinationAirportCode", "Destination Airport Code", RuleFieldDataType.String, _departureOnly, f => (f as DepartureFlight)?.DestinationAirportCode),
        Condition("OriginAirportCode", "Origin Airport Code", RuleFieldDataType.String, _arrivalOnly, f => (f as ArrivalFlight)?.OriginAirportCode),
        Condition("PaxTotal", "Total Passengers", RuleFieldDataType.Int, _both, f => f.LoadInfo.PaxTotal),
        Condition("PaxTransit", "Transit Passengers", RuleFieldDataType.Int, _both, f => f.LoadInfo.PaxTransit),
        Condition("PaxAssistanceRequired", "Assistance Required Passengers", RuleFieldDataType.Int, _both, f => f.LoadInfo.PaxAssistanceRequired),
        // Pax1/Pax2/Pax3 are the standard IATA/ICAO load-control passenger-type breakdown — see
        // FlightEditPanel's "Load & Pax" tab, labeled the same way there.
        Condition("Pax1", "Adults", RuleFieldDataType.Int, _both, f => f.LoadInfo.Pax1),
        Condition("Pax2", "Children", RuleFieldDataType.Int, _both, f => f.LoadInfo.Pax2),
        Condition("Pax3", "Infants", RuleFieldDataType.Int, _both, f => f.LoadInfo.Pax3),
        Condition("CrewNumber", "Crew Number", RuleFieldDataType.Int, _both, f => f.LoadInfo.CrewNumber),
        Condition("CrewCockpit", "Cockpit Crew", RuleFieldDataType.Int, _both, f => f.LoadInfo.CrewCockpit),
        Condition("BaggageWeight", "Baggage Weight", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.BaggageWeight),
        Condition("BaggageTotal", "Baggage Total", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.BaggageTotal),
        Condition("CargoWeight", "Cargo Weight", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.CargoWeight),
        Condition("MailWeight", "Mail Weight", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.MailWeight),
        Condition("TotalWeight", "Total Weight", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.TotalWeight),
        Condition("EquipmentWeight", "Equipment Weight", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.EquipmentWeight),
        Condition("CabinLuggage", "Cabin Luggage", RuleFieldDataType.Decimal, _both, f => f.LoadInfo.CabinLuggage),

        // ── Fields usable in both condition and action ──
        // All of these must pass _setOrAppend explicitly — SupportedActionTypes has no other
        // default here (unlike the Action() helper below), and null reads client-side as "valid
        // for every action type," which let these leak into the Target Field list even when the
        // selected Action Type was Require Resource Count.
        new FlightRuleField("TerminalCode", "Terminal Code", RuleFieldDataType.String, _both, true, true,
            f => f.TerminalCode, (f, v, _, _) => f.UpdateTerminal(v), _setOrAppend),
        new FlightRuleField("CountryType", "Country Type", RuleFieldDataType.String, _both, true, true,
            f => f.CountryType, (f, v, _, _) => f.UpdateCountryType(v ?? string.Empty), _setOrAppend),
        // Raw status assignment — bypasses Cancel()'s CancellationDateTime side effect, so a rule
        // that sets Status to "XLD" won't stamp a cancellation time the way the Cancel flow does.
        new FlightRuleField("Status", "Status", RuleFieldDataType.String, _both, true, true,
            f => f.Status, (f, v, _, _) => f.FlightUpdateStatus(v ?? f.Status), _setOrAppend),
        new FlightRuleField("IsGeneralAviation", "Is General Aviation", RuleFieldDataType.Bool, _both, true, true,
            f => f.IsGeneralAviation, (f, v, _, _) => f.UpdateGeneralAviation(RuleEvaluator.NormalizeBool(v) == "true"),
            [RuleActionType.SetField]),
        new FlightRuleField("FlightTypeCode", "Flight Type Code", RuleFieldDataType.String, _both, true, true,
            f => f.FlightTypeCode, (f, v, _, _) => f.UpdateFlightTypeCode(v ?? string.Empty), _setOrAppend),
        new FlightRuleField("OperatingAirlineCode", "Operating Airline Code", RuleFieldDataType.String, _both, true, true,
            f => f.OperatingAirlineCode, (f, v, _, _) => f.UpdateAircraft(f.AircraftCode, f.AircraftTypeCode, v, f.AircraftOwner), _setOrAppend),
        // Aircraft fields share one domain method (UpdateAircraft) that takes all four together —
        // pass the flight's current values through for the two the rule isn't targeting, or
        // they'd get wiped out instead of left alone.
        new FlightRuleField("AircraftCode", "Aircraft Registration", RuleFieldDataType.String, _both, true, true,
            f => f.AircraftCode, (f, v, _, _) => f.UpdateAircraft(v, f.AircraftTypeCode, f.OperatingAirlineCode, f.AircraftOwner), _setOrAppend),
        new FlightRuleField("AircraftTypeCode", "Aircraft Type Code", RuleFieldDataType.String, _both, true, true,
            f => f.AircraftTypeCode, (f, v, _, _) => f.UpdateAircraft(f.AircraftCode, v, f.OperatingAirlineCode, f.AircraftOwner), _setOrAppend),

        // ── Action-writable fields ──
        // Gates/Belts/Counters are read-only conditions now — they're computed straight from the
        // actual OpsGate/OpsBelt/OpsCounter resource rows (see ResourceRollup), so there's nothing
        // left to Set/Append; use RequiredGates/RequiredBelts/RequiredCounters below to actually
        // create resources, or the Resources tab / Resource endpoints for the real thing.
        Condition("Gates", "Gates", RuleFieldDataType.String, _both,
            f => f switch { ArrivalFlight a => a.Gates, DepartureFlight d => d.Gates, _ => null }),
        Condition("Counters", "Check-in Counters", RuleFieldDataType.String, _departureOnly,
            f => (f as DepartureFlight)?.Counters),
        Condition("Belts", "Baggage Belts", RuleFieldDataType.String, _both,
            f => f switch { ArrivalFlight a => a.Belts, DepartureFlight d => d.Belts, _ => null }),
        // ParkPositionCode/HandlingAgentCode are still on the shared Flight base — no cast needed, and
        // casting to DepartureFlight (as this used to) silently dropped them for arrivals too.
        Action("ParkPositionCode", "ParkPosition / ParkPosition Code", _both, f => f.ParkPositionCode, (f, v, _, _) => f.UpdateParkPosition(v ?? string.Empty, f.CurrentParkPositionCode)),
        Action("CurrentParkPositionCode", "Current Park Position Code", _both, f => f.CurrentParkPositionCode, (f, v, _, _) => f.UpdateParkPosition(f.ParkPositionCode ?? string.Empty, v)),
        Action("HandlingAgentCode", "Handling Agent Code", _both, f => f.HandlingAgentCode, (f, v, _, _) => f.UpdateHandling(v)),
        Action("CallSign", "Call Sign", _both, f => f.CallSign, (f, v, _, _) => f.UpdateCallSign(v)),
        Action("Comment", "Comment", _both, f => f.Comment, (f, v, _, _) => f.UpdateComment(v)),
        Action("DivertAirportCode", "Divert Airport Code", _arrivalOnly, f => (f as ArrivalFlight)?.DivertAirportCode,
            (f, v, _, _) => { if (f is ArrivalFlight a) a.UpdateOrigin(a.OriginAirportCode, v); }),

        // ── Resource-count actions — top up to N placeholder resources (e.g. "4" counters for a
        // B737), each stamped with the tenant's configured default plan window: opens
        // DefaultOffsetMinutes before the flight's own Sta/Std, stays open for
        // DefaultDurationMinutes — see ResourceDefaults/AeroConfigSeeder
        // ("Resources.{Kind}.DefaultOffsetMinutes"/"...DefaultDurationMinutes"/"...DefaultStatusCode").
        // Code stays unset; which physical resource is assigned is still left to the operator via
        // the Resources tab. Idempotent — only adds enough to reach the target count, so it's safe
        // on Updated triggers too (re-matching on every save won't pile up duplicates). Only usable
        // with RequireResourceCount — a plain SetField/AppendField on a count doesn't mean
        // anything, so they're excluded from those action types' Target Field options. ──
        Action("RequiredCounters", "Required Counters (count)", _departureCountable,
            f => (f as DepartureFlight)?.OpsCounters.Count,
            (f, v, _, defaults) => { if (f is DepartureFlight d) RequireCount(v, d.OpsCounters.Count, d.AddCounter, d.Std, defaults, "Counter"); },
            [RuleActionType.RequireResourceCount]),
        Action("RequiredBelts", "Required Belts (count)", _bothCountable,
            f => f switch { ArrivalFlight a => a.OpsBelts.Count, DepartureFlight d => d.OpsBelts.Count, _ => null },
            (f, v, _, defaults) =>
            {
                if (f is ArrivalFlight a) RequireCount(v, a.OpsBelts.Count, a.AddBelt, a.Sta, defaults, "ArrivalBelt");
                else if (f is DepartureFlight d) RequireCount(v, d.OpsBelts.Count, d.AddBelt, d.Std, defaults, "DepartureBelt");
            },
            [RuleActionType.RequireResourceCount]),
        Action("RequiredGates", "Required Gates (count)", _bothCountable,
            f => f switch { ArrivalFlight a => a.OpsGates.Count, DepartureFlight d => d.OpsGates.Count, _ => null },
            (f, v, _, defaults) =>
            {
                if (f is ArrivalFlight a) RequireCount(v, a.OpsGates.Count, a.AddGate, a.Sta, defaults, "ArrivalGate");
                else if (f is DepartureFlight d) RequireCount(v, d.OpsGates.Count, d.AddGate, d.Std, defaults, "DepartureGate");
            },
            [RuleActionType.RequireResourceCount]),
        // OpsFlightParkPosition isn't a ResourceBase (no StatusCode, plan window is required at
        // creation instead of set after) — separate helper below instead of forcing it through
        // RequireCount<TResource>.
        Action("RequiredParkPositions", "Required Park Positions (count)", _bothCountable,
            f => f switch { ArrivalFlight a => a.ParkPositions.Count, DepartureFlight d => d.ParkPositions.Count, _ => null },
            (f, v, _, defaults) =>
            {
                if (f is ArrivalFlight a) RequireParkPositionCount(v, a.ParkPositions.Count, a.AddParkPosition, a.Sta, defaults);
                else if (f is DepartureFlight d) RequireParkPositionCount(v, d.ParkPositions.Count, d.AddParkPosition, d.Std, defaults);
            },
            [RuleActionType.RequireResourceCount]),

        // ── Finance actions — auto-assign a payer (company code) to every resource of the given
        // kind currently on the flight that doesn't already have one. Fires on the already-wired
        // Flight save triggers (_bothCountable/_departureCountable) rather than needing per-resource
        // Created triggers, and only fills in a missing payer — never overwrites one set manually
        // from the Finance tab, so it's safe to re-run on every update. ──
        Action("GatePayerCode", "Gate Payer (Company Code)", _bothCountable,
            f => null, (f, v, _, _) => StampPayer(GatesOf(f), v), [RuleActionType.Finance]),
        Action("CounterPayerCode", "Counter Payer (Company Code)", _departureCountable,
            f => null, (f, v, _, _) => StampPayer(CountersOf(f), v), [RuleActionType.Finance]),
        Action("BeltPayerCode", "Belt Payer (Company Code)", _bothCountable,
            f => null, (f, v, _, _) => StampPayer(BeltsOf(f), v), [RuleActionType.Finance]),
        Action("ParkPositionPayerCode", "Park Position Payer (Company Code)", _bothCountable,
            f => null, (f, v, _, _) => StampParkPositionPayer(ParkPositionsOf(f), v), [RuleActionType.Finance]),
        // Handling services vary in who pays per service code (e.g. GPU billed to the handler,
        // security billed elsewhere) — unlike the other Finance targets, this one supports the
        // action's Match Value: leave it blank to pay every service code, or set it to a specific
        // service code (e.g. "GPU") to scope this action to just that one. Add one action per
        // service code that needs its own payer, plus an optional action with no Match Value as a
        // catch-all default for anything not covered by a more specific one.
        new FlightRuleField("ServicePayerCode", "Handling Service Payer (Company Code)", RuleFieldDataType.String, _bothCountable, false, true,
            f => null, (f, v, matchServiceCode, _) => StampServicePayer(ServicesOf(f), v, matchServiceCode),
            [RuleActionType.Finance], SupportsMatchValue: true, MatchValueLabel: "Service Code (blank = all)"),

        // ── Validation action — no flight field to read or write; when its conditions match, it
        // blocks the save entirely with an operator-authored message (RuleEvaluator.ApplyAction
        // throws before ever reaching Getter/Setter here). This field exists only so RaiseError has
        // a Target Field entry like every other action type, for the UI dropdown and trigger checks. ──
        Action("ValidationError", "Error Message", _both, f => null, (f, v, _, _) => { }, [RuleActionType.RaiseError]),
    ];

    private static IEnumerable<ResourceBase> GatesOf(Flight f) => f switch
    {
        ArrivalFlight a => a.OpsGates,
        DepartureFlight d => d.OpsGates,
        _ => []
    };

    private static IEnumerable<ResourceBase> CountersOf(Flight f) =>
        f is DepartureFlight d ? d.OpsCounters : [];

    private static IEnumerable<ResourceBase> BeltsOf(Flight f) => f switch
    {
        ArrivalFlight a => a.OpsBelts,
        DepartureFlight d => d.OpsBelts,
        _ => []
    };

    private static IEnumerable<OpsService> ServicesOf(Flight f) => f switch
    {
        ArrivalFlight a => a.Services,
        DepartureFlight d => d.Services,
        _ => []
    };

    private static IEnumerable<OpsFlightParkPosition> ParkPositionsOf(Flight f) => f switch
    {
        ArrivalFlight a => a.ParkPositions,
        DepartureFlight d => d.ParkPositions,
        _ => []
    };

    private static void StampPayer(IEnumerable<ResourceBase> resources, string? payer)
    {
        if (string.IsNullOrWhiteSpace(payer)) return;
        foreach (var r in resources)
            if (string.IsNullOrWhiteSpace(r.PayerCompanyCode))
                r.UpdatePayer(payer);
    }

    // matchServiceCode comes from the action's Match Value — null/blank applies to every service
    // code, otherwise only services whose ServiceCode equals it (case-insensitive) get stamped.
    private static void StampServicePayer(IEnumerable<OpsService> services, string? payer, string? matchServiceCode)
    {
        if (string.IsNullOrWhiteSpace(payer)) return;

        foreach (var s in services)
        {
            if (!string.IsNullOrWhiteSpace(s.PayerCompanyCode)) continue;
            if (!string.IsNullOrWhiteSpace(matchServiceCode) && !string.Equals(s.ServiceCode, matchServiceCode, StringComparison.OrdinalIgnoreCase)) continue;

            s.UpdatePayer(payer);
        }
    }

    private static void StampParkPositionPayer(IEnumerable<OpsFlightParkPosition> positions, string? payer)
    {
        if (string.IsNullOrWhiteSpace(payer)) return;
        foreach (var p in positions)
            if (string.IsNullOrWhiteSpace(p.PayerCompanyCode))
                p.UpdatePayer(payer);
    }

    private static void RequireCount<TResource>(
        string? value, int currentCount, Func<string?, TResource> addOne, DateTimeOffset anchor, ResourceDefaults defaults, string kind)
        where TResource : ResourceBase
    {
        if (!int.TryParse(value, out var count))
            return;

        var missing = count - currentCount;
        if (missing <= 0)
            return;

        var begin = anchor.AddMinutes(-defaults.OffsetMinutes(kind));
        var end = begin.AddMinutes(defaults.DurationMinutes(kind));
        var statusCode = defaults.StatusCode(kind);

        for (var i = 0; i < missing; i++)
        {
            var resource = addOne(null);
            resource.SetPlan(begin, end);
            resource.UpdateStatus(statusCode);
        }
    }

    private static void RequireParkPositionCount(
        string? value, int currentCount, Func<DateTimeOffset, DateTimeOffset, OpsFlightParkPosition> addOne,
        DateTimeOffset anchor, ResourceDefaults defaults)
    {
        if (!int.TryParse(value, out var count))
            return;

        var missing = count - currentCount;
        if (missing <= 0)
            return;

        var begin = anchor.AddMinutes(-defaults.OffsetMinutes("ParkPosition"));
        var end = begin.AddMinutes(defaults.DurationMinutes("ParkPosition"));

        for (var i = 0; i < missing; i++)
            addOne(begin, end);
    }

    private static FlightRuleField Condition(string key, string label, RuleFieldDataType dataType,
        IReadOnlyList<RuleTrigger> triggers, Func<Flight, object?> getter)
        => new(key, label, dataType, triggers, true, false, getter, null);

    private static readonly RuleActionType[] _setOrAppend = [RuleActionType.SetField, RuleActionType.AppendField];

    private static FlightRuleField Action(string key, string label, IReadOnlyList<RuleTrigger> triggers,
        Func<Flight, object?> getter, Action<Flight, string?, string?, ResourceDefaults> setter, IReadOnlyList<RuleActionType>? supportedActionTypes = null)
        => new(key, label, RuleFieldDataType.String, triggers, false, true, getter, setter, supportedActionTypes ?? _setOrAppend);
}
