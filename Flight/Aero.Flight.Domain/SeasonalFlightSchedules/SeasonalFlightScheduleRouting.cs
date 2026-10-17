using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.SeasonalFlightSchedules;

public abstract class SeasonalFlightScheduleRouting : AuditBase<int>
{
    public int SeasonalFlightScheduleFlightId { get; private set; }
    public string AirportCode { get; private set; } = string.Empty;
    public int LegNumber { get; private set; }
    public string DirectionCode { get; private set; } = string.Empty;
    public DateTimeOffset? Sta { get; private set; }
    public DateTimeOffset? Std { get; private set; }

    protected SeasonalFlightScheduleRouting() { }

    protected SeasonalFlightScheduleRouting(int seasonalFlightScheduleFlightId, string airportCode, int legNumber, string directionCode)
    {
        SeasonalFlightScheduleFlightId = seasonalFlightScheduleFlightId;
        AirportCode = airportCode;
        LegNumber = legNumber;
        DirectionCode = directionCode;
    }

    public void UpdateTimes(DateTimeOffset? sta, DateTimeOffset? std)
    {
        Sta = sta;
        Std = std;
    }
}
