using Domain.Aggregates.Companies;
using Domain.Aggregates.Reconciliations;
using Domain.Common;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Aggregates.Flights;

public sealed class DepartureFlight : Entity<int>, IFlight
{
    private readonly List<DepartureFlightReconciliation> _reconciliationRecords = [];
    public string  DestinationAirport { get; private set; } = string.Empty;
    public string  NextAirport        { get; private set; } = string.Empty;
    public string? Gate               { get; private set; }
    public string? GateStatus         { get; private set; }
    public string? CheckIn            { get; private set; }
    public string? CheckInStatus      { get; private set; }
    public string? Chute              { get; private set; }
    
    public DepartureFlightStatus FlightStatus { get; private set; }

    
    public string RemoteSystemId { get; private set; } = default!;

    public string AirlineCode { get; private set; } = default!;

    public string FlightNumber { get; private set; } = default!;

    public DateTime ScheduledDateTime { get; private set; }

    public DateTime? EstimatedDateTime { get; private set; }

    public DateTime? ActualDateTime { get; private set; }

    public string FlightIataDate { get; private set; } = default!;

    public string IntDom { get; private set; } = default!;

    public string? Terminal { get; private set; }

    public string? Registration { get; private set; }

    public string? ParkingPosition { get; private set; }

    public int TotalPassengers { get; private set; }

    public string? HandlingCompanyCode { get; private set; }

    public Company? HandlingCompany { get; private set; }
    private DepartureFlight() { }

    public static DepartureFlight Create(
        string remoteSystemId,
        string airlineCode,
        string flightNumber,
        string nextAirport,
        string destinationAirport,
        DateTime scheduledDeparture,
        DateTime? estimatedDeparture)
    {
        DepartureFlight departureFlight = new()
        {
            NextAirport = nextAirport,
            DestinationAirport = destinationAirport
        };
        departureFlight.SetFlightData(airlineCode, flightNumber, scheduledDeparture);
        departureFlight.SetOperationsDateTime(scheduledDeparture, estimatedDeparture);
        departureFlight.SetRemoteSystemId(remoteSystemId);
        departureFlight.SetFlightStatus(DepartureFlightStatus.Scheduled);
        return departureFlight;
    }

    public void SetFlightStatus(DepartureFlightStatus flightStatus) => FlightStatus = flightStatus;
    public void SetHandlingCompany(string handlingCode) => HandlingCompanyCode = handlingCode;
    public void SetFlightData(string airlineCode, string flightNumber, DateTime scheduledDateTime)
    {
        AirlineCode = airlineCode;
        FlightNumber = flightNumber;
        ScheduledDateTime = scheduledDateTime;
        FlightIataDate = scheduledDateTime.ToString("ddMMM").ToUpperInvariant();
    }
    public void SetRemoteSystemId(string remoteSystemId) => RemoteSystemId = remoteSystemId;
    public void SetOperationsDateTime(DateTime scheduledDeparture, DateTime? estimatedDateTime)
    {
        ScheduledDateTime = scheduledDeparture;
        FlightIataDate =  scheduledDeparture.ToString("ddMMM").ToUpperInvariant();
        EstimatedDateTime = estimatedDateTime;
    }
    public void SetParkingPosition(string parkingPosition) => ParkingPosition = parkingPosition;
    public void SetRegistration(string registration) => Registration = registration;
    public void SetIntDom(string intdom) => IntDom = intdom;
    public void SetTerminal(string terminal) => Terminal = terminal;
    public void SetActualDateTime(DateTime actualDateTime) => ActualDateTime = actualDateTime;
    public void SetChute(string chute) => Chute = chute;

    public void SetGate(string gate, string status)
    {
        Gate       = gate;
        GateStatus = status;
    }

    public void SetCheckin(string checkin, string status)
    {
        CheckIn       = checkin;
        CheckInStatus = status;
    }

    public string Destination => NextAirport == DestinationAirport ? NextAirport : $"{NextAirport}/{DestinationAirport}";

    public IReadOnlyList<DepartureFlightReconciliation> ReconciliationRecords => _reconciliationRecords.AsReadOnly();

    public DepartureFlightReconciliation? LatestReconciliation =>
        _reconciliationRecords.Count > 0
            ? _reconciliationRecords.OrderByDescending(r => r.SnapshotAt).First()
            : null;

}
