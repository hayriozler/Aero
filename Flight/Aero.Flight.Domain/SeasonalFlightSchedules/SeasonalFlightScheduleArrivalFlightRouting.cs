namespace Aero.Flights.Domain.SeasonalFlightSchedules;

public sealed class SeasonalFlightScheduleArrivalFlightRouting : SeasonalFlightScheduleRouting
{
    public SeasonalFlightScheduleArrivalFlight? Flight { get; private set; }

    private SeasonalFlightScheduleArrivalFlightRouting() { }

    private SeasonalFlightScheduleArrivalFlightRouting(int seasonalFlightScheduleFlightId, string airportCode, int legNumber, string directionCode)
        : base(seasonalFlightScheduleFlightId, airportCode, legNumber, directionCode) { }

    public static SeasonalFlightScheduleArrivalFlightRouting Create(int seasonalFlightScheduleFlightId, string airportCode, int legNumber, string directionCode)
        => new(seasonalFlightScheduleFlightId, airportCode, legNumber, directionCode);
}
