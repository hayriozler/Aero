using Domain.Aggregates.Companies;
using Domain.Aggregates.Reconciliations;
using Domain.Common;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Aggregates.Flights;

public sealed class ArrivalFlight : Entity<int>, IFlight
{
    public string  OriginAirport   { get; private set; } = string.Empty;
    public string  PreviousAirport { get; private set; } = string.Empty;
    public string? Carousel        { get; private set; }

    public ArrivalFlightStatus FlightStatus { get; private set; }

    private readonly List<ArrivalFlightReconciliation> _reconciliationRecords = [];


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

    private ArrivalFlight() { }
    public static ArrivalFlight Create(
        string remoteSystemId,
        string airlineCode,
        string flightNumber,
        string previousAirport,
        string originAirport,
        DateTime scheduledDateTime,
        DateTime? estimatedDateTime)
    {
        ArrivalFlight arrivalFlight = new()
        {
            PreviousAirport = previousAirport,
            OriginAirport = originAirport
        };
        arrivalFlight.SetRemoteSystemId(remoteSystemId);
        arrivalFlight.SetFlightData(airlineCode, flightNumber, scheduledDateTime);
        arrivalFlight.SetOperationsDateTime(scheduledDateTime, estimatedDateTime);
        arrivalFlight.SetFlightStatus(ArrivalFlightStatus.Scheduled);
        return arrivalFlight;
    }

    public void SetFlightStatus(ArrivalFlightStatus flightStatus) => FlightStatus = flightStatus;
    public void SetCarousel(string carousel) => Carousel = carousel;
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
    public string Destination => PreviousAirport == OriginAirport ? OriginAirport : $"{OriginAirport}/{PreviousAirport}";

    public IReadOnlyList<ArrivalFlightReconciliation> ReconciliationRecords => _reconciliationRecords.AsReadOnly();

    public ArrivalFlightReconciliation? LatestReconciliation =>
        _reconciliationRecords.Count > 0
            ? _reconciliationRecords.OrderByDescending(r => r.SnapshotAt).First()
            : null;


}
