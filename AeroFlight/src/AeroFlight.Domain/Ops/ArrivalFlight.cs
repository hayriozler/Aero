using AeroFlight.Domain.Ops.Delays;
using AeroFlight.Domain.Ops.Passengers;
using AeroFlight.Domain.Ops.Resources;
using AeroFlight.Domain.Ops.Routing;
using AeroFlight.Domain.Ops.Services;

namespace AeroFlight.Domain.Ops;

public sealed class ArrivalFlight : Flight
{
    private readonly List<OpsArrivalPassenger> _passengers = [];
    private readonly List<OpsArrivalBelt> _opsBelts = [];
    private readonly List<OpsArrivalGate> _opsGates = [];
    private readonly List<OpsArrivalRouting> _routings = [];
    private readonly List<OpsArrivalService> _services = [];
    private readonly List<OpsArrivalCodeShare> _codeShares = [];
    private readonly List<OpsArrivalDelay> _delays = [];
    private readonly List<OpsFlightParkPosition> _parkPositions = [];
    public long? DepartureFlightId { get; private set; }
    public DateTimeOffset Sta { get; private set; }
    public DateTimeOffset? Eta { get; private set; }
    public DateTimeOffset? Ata { get; private set; }
    public DateTimeOffset? TouchDown { get; private set; }
    public DateTimeOffset? OnBlock { get; private set; }
    public string OriginAirportCode => _routings.FirstOrDefault(r => r.LegNumber == 0)?.Code ?? string.Empty;
    public string? DivertAirportCode { get; private set; }
    public bool IsDivertedFlight { get; private set; }
    // Derived from the actual OpsArrivalGate/OpsArrivalBelt rows — not stored — so there's no way
    // for these to drift from what's really assigned. See ResourceRollup.
    public string? Gates => ResourceRollup.Codes(_opsGates);
    public string? GateStatusCode => ResourceRollup.StatusCode(_opsGates);
    public DateTimeOffset? DeboardingBegin => ResourceRollup.Begin(_opsGates);
    public DateTimeOffset? DeboardingEnd => ResourceRollup.End(_opsGates);
    public string? Belts => ResourceRollup.Codes(_opsBelts);
    public string? BeltStatusCode => ResourceRollup.StatusCode(_opsBelts);
    public DateTimeOffset? BeltBegin => ResourceRollup.Begin(_opsBelts);
    public DateTimeOffset? BeltLastBag { get; private set; }
    public DateTimeOffset? BeltEnd => ResourceRollup.End(_opsBelts);
    public DateTimeOffset FlightDateTime => Eta ?? Sta;

    // Quick-glance summary of the routing legs so a user doesn't need to open the leg table to
    // see the extent of a multi-sector itinerary. Arrivals read backwards in time — higher leg
    // numbers are further back in the rotation, leg 0 (Origin) is the immediate prior stop — so
    // the oldest leg is "first" and Origin is "last", collapsed to one code when they're the same.
    public string? RoutingSummary
    {
        get
        {
            if (_routings.Count == 0)
                return null;

            var ordered = _routings.OrderByDescending(r => r.LegNumber).ToList();
            var first = ordered[0].Code;
            var last = ordered[^1].Code;
            return first == last ? first : $"{first} / {last}";
        }
    }

    public IReadOnlyCollection<OpsArrivalPassenger> Passengers => _passengers;
    public IReadOnlyCollection<OpsArrivalBelt> OpsBelts => _opsBelts;
    public IReadOnlyCollection<OpsArrivalGate> OpsGates => _opsGates;
    public IReadOnlyCollection<OpsArrivalRouting> Routings => _routings;
    public IReadOnlyCollection<OpsArrivalService> Services => _services;
    public IReadOnlyCollection<OpsArrivalCodeShare> CodeShares => _codeShares;
    public IReadOnlyCollection<OpsArrivalDelay> Delays => _delays;
    public IReadOnlyCollection<OpsFlightParkPosition> ParkPositions => _parkPositions;
    private ArrivalFlight() { }

    private ArrivalFlight(string airlineCode, string flightNumber, DateTimeOffset sta, string originAirportCode)
        : base(airlineCode, flightNumber)
    {
        Sta = sta;
        UpdateFlightDate(sta);
        _routings.Add(OpsArrivalRouting.Create(Id, originAirportCode, 0));
    }

    public static ArrivalFlight Create(string airlineCode, string flightNumber, DateTimeOffset sta, string originAirportCode)
        => new(airlineCode, flightNumber, sta, originAirportCode);

    public void UpdateSta(DateTimeOffset sta)
    {
        if (Sta == sta) return;
        Sta = sta;
        UpdateFlightDate(sta);
    }
    public void UpdateEta(DateTimeOffset? eta)
    {
        if (Eta == eta) return;
        Eta = eta;
    }
    public void RecordTouchDown(DateTimeOffset? touchDown)
    {
        if (TouchDown == touchDown) return;
        TouchDown = touchDown;
    }
    public void RecordAta(DateTimeOffset ata)
    {
        if (Ata == ata) return;
        Ata = ata;
    }
    public void RecordOnBlock(DateTimeOffset onBlock)
    {
        if (OnBlock == onBlock) return;
        OnBlock = onBlock;
    }

    public void LinkDepartureFlight(long departureFlightId)
    {
        if (DepartureFlightId == departureFlightId) return;
        DepartureFlightId = departureFlightId;
    }
    public void ClearDepartureLink()
    {
        if (DepartureFlightId is null) return;
        DepartureFlightId = null;
    }
    public void UpdateOrigin(string originAirportCode, string? divertAirportCode = null)
    {
        var anchor = _routings.FirstOrDefault(r => r.LegNumber == 0);
        if (anchor is null)
            _routings.Add(OpsArrivalRouting.Create(Id, originAirportCode, 0));
        else if (anchor.Code != originAirportCode)
            anchor.UpdateLeg(originAirportCode, 0);

        if (DivertAirportCode != divertAirportCode)
            DivertAirportCode = divertAirportCode;
    }

    // Additional leg beyond Origin — e.g. the "Previous" stop entered at creation time, or any
    // further-back leg of a multi-sector rotation. LegNumber must be > 0; 0 is reserved for Origin.
    public void AddRouting(string code, int legNumber)
        => _routings.Add(OpsArrivalRouting.Create(Id, code, legNumber));

    public void UpdateIsDivertedFlight(bool isDivertedFlight)
    {
        if (IsDivertedFlight == isDivertedFlight) return;
        IsDivertedFlight = isDivertedFlight;
    }

    public void AddCodeShare(string airlineCode, string flightNumber)
        => _codeShares.Add(OpsArrivalCodeShare.Create(Id, airlineCode, flightNumber));

    // Code-less placeholder — a rule requires N of this resource, but which physical
    // belt/gate is assigned is left to the operator via the Resources tab. Returns the created row
    // so the caller can apply tenant-configured defaults (plan window, status) on top.
    public OpsArrivalBelt AddBelt(string? code = null)
    {
        var belt = OpsArrivalBelt.Create(Id, code);
        _opsBelts.Add(belt);
        return belt;
    }

    public OpsArrivalGate AddGate(string? code = null)
    {
        var gate = OpsArrivalGate.Create(Id, code);
        _opsGates.Add(gate);
        return gate;
    }

    public OpsFlightParkPosition AddParkPosition(DateTimeOffset beginPlan, DateTimeOffset endPlan)
    {
        var parkPosition = OpsFlightParkPosition.CreateForArrival(Id, beginPlan, endPlan, _parkPositions.Count + 1);
        _parkPositions.Add(parkPosition);
        return parkPosition;
    }
}
