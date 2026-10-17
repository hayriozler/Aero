namespace Aero.Flights.Domain.SeasonalFlightSchedules;

public sealed class SeasonalFlightScheduleDepartureFlightRouting : SeasonalFlightScheduleRouting
{
    public SeasonalFlightScheduleDepartureFlight? Flight { get; private set; }

    private SeasonalFlightScheduleDepartureFlightRouting() { }

    private SeasonalFlightScheduleDepartureFlightRouting(int seasonalFlightScheduleFlightId, string airportCode, int legNumber, string directionCode)
        : base(seasonalFlightScheduleFlightId, airportCode, legNumber, directionCode) { }

    public static SeasonalFlightScheduleDepartureFlightRouting Create(int seasonalFlightScheduleFlightId, string airportCode, int legNumber, string directionCode)
        => new(seasonalFlightScheduleFlightId, airportCode, legNumber, directionCode);
}
