using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Ops.Passengers;

public sealed class OpsTransferPassenger : AuditBase<long>
{
    public long? ArrivalFlightId { get; private set; }
    public long? DepartureFlightId { get; private set; }
    public long? ImportMessageId { get; private set; }
    public string? FlightElement { get; private set; }
    public string ArrFlightNumber { get; private set; } = string.Empty;
    public DateTimeOffset ArrDate { get; private set; }
    public string DepFlightNumber { get; private set; } = string.Empty;
    public DateTimeOffset DepDate { get; private set; }
    public string? DepDestination { get; private set; }
    public int NumberOfSeats { get; private set; } = 1;
    public string PassengerNames { get; private set; } = string.Empty;
    public string? BookingClass { get; private set; }
    public int NumberOfBags { get; private set; }
    public decimal BagWeight { get; private set; }
    public bool IsConfirmed { get; private set; }
    public string? Comment { get; private set; }

    private OpsTransferPassenger() { }

    private OpsTransferPassenger(string arrFlightNumber, DateTimeOffset arrDate,
        string depFlightNumber, DateTimeOffset depDate, string passengerNames)
    {
        ArrFlightNumber = arrFlightNumber;
        ArrDate = arrDate;
        DepFlightNumber = depFlightNumber;
        DepDate = depDate;
        PassengerNames = passengerNames;
    }

    public static OpsTransferPassenger Create(string arrFlightNumber, DateTimeOffset arrDate,
        string depFlightNumber, DateTimeOffset depDate, string passengerNames)
        => new(arrFlightNumber, arrDate, depFlightNumber, depDate, passengerNames);

    public void LinkFlights(long? arrivalFlightId, long? departureFlightId)
    {
        ArrivalFlightId = arrivalFlightId;
        DepartureFlightId = departureFlightId;
    }

    public void UpdateBaggage(int numberOfBags, decimal bagWeight)
    {
        NumberOfBags = numberOfBags;
        BagWeight = bagWeight;
    }

    public void UpdateDetails(string? bookingClass, int numberOfSeats, string? depDestination, string? comment)
    {
        BookingClass = bookingClass;
        NumberOfSeats = numberOfSeats;
        DepDestination = depDestination;
        Comment = comment;
    }

    public void Confirm() => IsConfirmed = true;
}
