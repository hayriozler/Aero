using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.SeasonalFlightSchedules;

public abstract class SeasonalFlightScheduleFlight : AuditBase<int>
{
    public string ScheduleSeasonCode { get; private set; } = string.Empty;
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;
    public string? CallSign { get; private set; }
    public string? ScheduleFlightStatusCode { get; private set; }
    public string? CountryType { get; private set; }
    public string? AircraftTypeCode { get; private set; }
    public string? OpsDays { get; private set; }
    public int Frequency { get; private set; }
    public bool IsOverMidnight { get; private set; }
    public string? DirectionCode { get; private set; }
    public bool IsTransit { get; private set; }
    public string? Comment { get; private set; }

    public ScheduleSeason? Season { get; private set; }

    protected SeasonalFlightScheduleFlight() { }

    protected SeasonalFlightScheduleFlight(string scheduleSeasonCode, string airlineCode, string flightNumber)
    {
        ScheduleSeasonCode = scheduleSeasonCode;
        AirlineCode = airlineCode;
        FlightNumber = flightNumber;
    }

    public void UpdateOperationalDays(string opsDays, int frequency, bool isOverMidnight = false)
    {
        OpsDays = opsDays;
        Frequency = frequency;
        IsOverMidnight = isOverMidnight;
    }

    public void UpdateStatus(string? statusCode) => ScheduleFlightStatusCode = statusCode;
    public void UpdateCallSign(string? callSign) => CallSign = callSign;
    public void UpdateCountryType(string? countryType) => CountryType = countryType;
    public void UpdateDirection(string? directionCode, bool isTransit)
    {
        DirectionCode = directionCode;
        IsTransit = isTransit;
    }
    public void UpdateAircraftType(string? aircraftTypeCode, string? acConfiguration = null) => AircraftTypeCode = aircraftTypeCode;
    public void UpdateComment(string? comment) => Comment = comment;
}
