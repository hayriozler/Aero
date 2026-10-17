using Aero.Flights.Domain.Ops.Delays;
using Aero.Flights.Domain.Ops.Passengers;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Ops.Routing;
using Aero.Flights.Domain.Ops.Services;

namespace Aero.Flights.Domain.Ops;

public sealed class DepartureFlight : Flight
{
    private readonly List<OpsDeparturePassenger> _passengers = [];
    private readonly List<OpsDepartureBelt> _opsBelts = [];
    private readonly List<OpsDepartureGate> _opsGates = [];
    private readonly List<OpsCounter> _opsCounters = [];
    private readonly List<OpsDepartureRouting> _routings = [];
    private readonly List<OpsDepartureService> _services = [];
    private readonly List<OpsDepartureCodeShare> _codeShares = [];
    private readonly List<OpsDepartureDelay> _delays = [];
    private readonly List<OpsFlightParkPosition> _parkPositions = [];

    public long? ArrivalFlightId { get; private set; }
    public DateTimeOffset Std { get; private set; }
    public DateTimeOffset? Etd { get; private set; }
    public DateTimeOffset? Atd { get; private set; }
    public DateTimeOffset? OffBlock { get; private set; }
    public DateTimeOffset? TakeOff { get; private set; }


    // Destination is not a stored scalar — it's the routing leg immediately after this departure
    // (LegNumber 0), so it lives in the same place as every other leg of the itinerary.
    public string DestinationAirportCode => _routings.FirstOrDefault(r => r.LegNumber == 0)?.Code ?? string.Empty;

    // Derived from the actual OpsDepartureGate/OpsDepartureBelt/OpsCounter rows — not stored — so
    // there's no way for these to drift from what's really assigned. See ResourceRollup.
    public string? Gates => ResourceRollup.Codes(_opsGates);
    public string? GateStatusCode => ResourceRollup.StatusCode(_opsGates);
    public DateTimeOffset? GateBegin => ResourceRollup.Begin(_opsGates);
    public DateTimeOffset? FinalCall { get; private set; }
    public DateTimeOffset? GateEnd => ResourceRollup.End(_opsGates);
    public string? Counters => ResourceRollup.Codes(_opsCounters);
    public string? CounterStatusCode => ResourceRollup.StatusCode(_opsCounters);
    public DateTimeOffset? CounterBegin => ResourceRollup.Begin(_opsCounters);
    public DateTimeOffset? CounterEnd => ResourceRollup.End(_opsCounters);
    public string? Belts => ResourceRollup.Codes(_opsBelts);
    public string? BeltStatusCode => ResourceRollup.StatusCode(_opsBelts);
    public DateTimeOffset? BeltBegin => ResourceRollup.Begin(_opsBelts);
    public DateTimeOffset? BeltEnd => ResourceRollup.End(_opsBelts);

    public DateTimeOffset FlightDateTime => Etd ?? Std;

    // Quick-glance summary of the routing legs so a user doesn't need to open the leg table to
    // see the extent of a multi-sector itinerary. Departures read forwards in time — leg 0
    // (Destination) is the immediate next stop, higher leg numbers are further ahead — so
    // Destination is "first" and the furthest leg is "last", collapsed to one code when they're the same.
    public string? RoutingSummary
    {
        get
        {
            if (_routings.Count == 0)
                return null;

            var ordered = _routings.OrderBy(r => r.LegNumber).ToList();
            var first = ordered[0].Code;
            var last = ordered[^1].Code;
            return first == last ? first : $"{first} / {last}";
        }
    }

    public IReadOnlyCollection<OpsDeparturePassenger> Passengers => _passengers;
    public IReadOnlyCollection<OpsDepartureBelt> OpsBelts => _opsBelts;
    public IReadOnlyCollection<OpsDepartureGate> OpsGates => _opsGates;
    public IReadOnlyCollection<OpsCounter> OpsCounters => _opsCounters;
    public IReadOnlyCollection<OpsDepartureRouting> Routings => _routings;
    public IReadOnlyCollection<OpsDepartureService> Services => _services;
    public IReadOnlyCollection<OpsDepartureCodeShare> CodeShares => _codeShares;
    public IReadOnlyCollection<OpsDepartureDelay> Delays => _delays;
    public IReadOnlyCollection<OpsFlightParkPosition> ParkPositions => _parkPositions;
    private DepartureFlight() { }

    private DepartureFlight(string airlineCode, string flightNumber, DateTimeOffset std, string destinationAirportCode)
        : base(airlineCode, flightNumber)
    {
        Std = std;
        UpdateFlightDate(std.Date);
        _routings.Add(OpsDepartureRouting.Create(Id, destinationAirportCode, 0));
    }

    public static DepartureFlight Create(string airlineCode, string flightNumber, DateTimeOffset std, string destinationAirportCode)
        => new(airlineCode, flightNumber, std, destinationAirportCode);

    public void UpdateStd(DateTimeOffset std)
    {
        if (Std == std) return;
        Std = std;
        UpdateFlightDate(std);
    }
    public void UpdateEtd(DateTimeOffset? etd)
    {
        if (Etd == etd) return;
        Etd = etd;
    }
    public void RecordAtd(DateTimeOffset atd)
    {
        if (Atd == atd) return;
        Atd = atd;
    }
    public void RecordOffBlock(DateTimeOffset offBlock)
    {
        if (OffBlock == offBlock) return;
        OffBlock = offBlock;
    }
    public void RecordTakeOff(DateTimeOffset takeOff)
    {
        if (TakeOff == takeOff) return;
        TakeOff = takeOff;
    }
    public void RecordFinalCall(DateTimeOffset finalCall)
    {
        if (FinalCall == finalCall) return;
        FinalCall = finalCall;
    }
    public void LinkArrivalFlight(long arrivalFlightId)
    {
        if (ArrivalFlightId == arrivalFlightId) return;
        ArrivalFlightId = arrivalFlightId;
    }
    public void ClearArrivalLink()
    {
        if (ArrivalFlightId is null) return;
        ArrivalFlightId = null;
    }

    public void UpdateDestination(string destinationAirportCode)
    {
        var anchor = _routings.FirstOrDefault(r => r.LegNumber == 0);
        if (anchor is null)
            _routings.Add(OpsDepartureRouting.Create(Id, destinationAirportCode, 0));
        else if (anchor.Code != destinationAirportCode)
            anchor.UpdateLeg(destinationAirportCode, 0);
    }

    // Additional leg beyond Destination — e.g. the "Next" stop entered at creation time, or any
    // further-ahead leg of a multi-sector rotation. LegNumber must be > 0; 0 is reserved for Destination.
    public void AddRouting(string code, int legNumber)
        => _routings.Add(OpsDepartureRouting.Create(Id, code, legNumber));

    public void AddCodeShare(string airlineCode, string flightNumber)
        => _codeShares.Add(OpsDepartureCodeShare.Create(Id, airlineCode, flightNumber));

    // Code-less placeholder — a rule requires N of this resource, but which physical
    // counter/belt/gate is assigned is left to the operator via the Resources tab. Returns the
    // created row so the caller can apply tenant-configured defaults (plan window, status) on top.
    public OpsCounter AddCounter(string? code = null)
    {
        var counter = OpsCounter.Create(Id, code);
        _opsCounters.Add(counter);
        return counter;
    }

    public OpsDepartureBelt AddBelt(string? code = null)
    {
        var belt = OpsDepartureBelt.Create(Id, code);
        _opsBelts.Add(belt);
        return belt;
    }

    public OpsDepartureGate AddGate(string? code = null)
    {
        var gate = OpsDepartureGate.Create(Id, code);
        _opsGates.Add(gate);
        return gate;
    }

    public OpsFlightParkPosition AddParkPosition(DateTimeOffset beginPlan, DateTimeOffset endPlan)
    {
        var parkPosition = OpsFlightParkPosition.CreateForDeparture(Id, beginPlan, endPlan, _parkPositions.Count + 1);
        _parkPositions.Add(parkPosition);
        return parkPosition;
    }
}
