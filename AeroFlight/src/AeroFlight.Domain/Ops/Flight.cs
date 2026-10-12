using AeroFlight.Domain.Common;
using AeroFlight.Domain.Metadatas;

namespace AeroFlight.Domain.Ops;

public abstract class Flight : AuditBase<long>
{
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;
    public DateOnly FlightDate { get; private set; }
    public string? CallSign { get; private set; }
    public int? CodeshareNumber { get; private set; }
    public string FlightTypeCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = FlightStatusCodes.Scheduled;
    public string? CountryType { get; private set; }
    public string? AircraftCode { get; private set; }
    public string? AircraftTypeCode { get; private set; }
    public string? OperatingAirlineCode { get; private set; }
    public string? AircraftOwner { get; private set; }
    public string? ParkPositionCode { get; private set; }
    public string? CurrentParkPositionCode { get; private set; }
    public string? TerminalCode { get; private set; }
    public string? HandlingAgentCode { get; private set; }
    public string? Comment { get; private set; }    
    public bool IsGeneralAviation { get; private set; }
    public DateTimeOffset? CancellationDateTime { get; private set; }

    // Passenger/crew counts and cargo weights — moved off Flight's own property list into their
    // own table (see FlightLoadInfo) since they're a distinct concern (the flight's load, not its
    // identity/schedule/status) and were the bulk of Flight's field count.
    public FlightLoadInfo LoadInfo { get; private set; } = new();

    protected Flight() { }

    protected Flight(string airlineCode, string flightNumber)
    {
        AirlineCode = airlineCode;
        FlightNumber = flightNumber;
    }

    public void FlightUpdateStatus(string status)
    {
        if (Status == status) return;
        Status = status;
    }
    internal void UpdateFlightDate(DateTimeOffset dateTime)
    {
        var dateOnly = DateOnly.FromDateTime(dateTime.Date);
        if (FlightDate == dateOnly) return;
        FlightDate = dateOnly;
    }
    public void UpdateFlightNumber(string flightNumber)
    {
        if (FlightNumber == flightNumber) return;
        FlightNumber = flightNumber;
    }

    public void UpdateParkPosition(string? parkPositionCode, string? currentParkPositionCode = null)
    {
        if (ParkPositionCode != parkPositionCode)
            ParkPositionCode = parkPositionCode;
        if (CurrentParkPositionCode != currentParkPositionCode)
            CurrentParkPositionCode = currentParkPositionCode;
    }

    public void Cancel(DateTimeOffset cancellationDateTime)
    {
        Status = FlightStatusCodes.Cancelled;
        CancellationDateTime = cancellationDateTime;
    }

    public void UpdateTerminal(string? terminalCode)
    {
        if (TerminalCode == terminalCode) return;
        TerminalCode = terminalCode;
    }

    public void UpdateCountryType(string countryType)
    {
        if (CountryType == countryType) return;
        CountryType = countryType;
    }

    public void UpdateHandling(string? handlingAgentCode)
    {
        if (HandlingAgentCode == handlingAgentCode) return;
        HandlingAgentCode = handlingAgentCode;
    }

    public void UpdateAircraft(string? aircraftCode, string? aircraftTypeCode = null,
        string? operatingAirlineCode = null, string? aircraftOwner = null)
    {
        if (AircraftCode == aircraftCode && AircraftTypeCode == aircraftTypeCode
            && OperatingAirlineCode == operatingAirlineCode && AircraftOwner == aircraftOwner)
            return;

        AircraftCode = aircraftCode;
        AircraftTypeCode = aircraftTypeCode;
        OperatingAirlineCode = operatingAirlineCode;
        AircraftOwner = aircraftOwner;
    }

    public void UpdateCallSign(string? callSign)
    {
        if (CallSign == callSign) return;
        CallSign = callSign;
    }

    public void UpdateCodeshareNumber(int? codeshareNumber)
    {
        if (CodeshareNumber == codeshareNumber) return;
        CodeshareNumber = codeshareNumber;
    }

    public void UpdateComment(string? comment)
    {
        if (Comment == comment) return;
        Comment = comment;
    }

    public void UpdateFlightTypeCode(string flightTypeCode)
    {
        if (FlightTypeCode == flightTypeCode) return;
        FlightTypeCode = flightTypeCode;
    }

    public void UpdateGeneralAviation(bool isGeneralAviation)
    {
        if (IsGeneralAviation == isGeneralAviation) return;
        IsGeneralAviation = isGeneralAviation;
    }

    public void UpdatePaxCounts(int? total, int? transit, int? assistanceRequired,
        int? pax1, int? pax2, int? pax3,
        int? crewNumber, int? crewCockpit)
        => LoadInfo.UpdateCounts(total, transit, assistanceRequired, pax1, pax2, pax3, crewNumber, crewCockpit);

    public void UpdateWeightData(decimal? baggageWeight, decimal? baggageTotal, decimal? cargoWeight,
        decimal? mailWeight, decimal? totalWeight, decimal? equipmentWeight, decimal? cabinLuggage)
        => LoadInfo.UpdateWeights(baggageWeight, baggageTotal, cargoWeight, mailWeight, totalWeight, equipmentWeight, cabinLuggage);
}
