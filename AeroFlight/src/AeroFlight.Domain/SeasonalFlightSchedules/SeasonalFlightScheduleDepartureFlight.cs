namespace AeroFlight.Domain.SeasonalFlightSchedules;

public sealed class SeasonalFlightScheduleDepartureFlight : SeasonalFlightScheduleFlight
{
    private readonly List<SeasonalFlightScheduleDepartureFlightRouting> _routings = [];

    public DateTimeOffset Std { get; private set; }
    public DateTimeOffset? Ptd { get; private set; }
    public DateTimeOffset PeriodBegin { get; private set; }
    public DateTimeOffset PeriodEnd { get; private set; }
    public string DestinationAirportCode { get; private set; } = string.Empty;
    public int? LinkedArrivalFlightId { get; private set; }

    public IReadOnlyCollection<SeasonalFlightScheduleDepartureFlightRouting> Routings => _routings;

    private SeasonalFlightScheduleDepartureFlight() { }

    private SeasonalFlightScheduleDepartureFlight(string scheduleSeasonCode, string airlineCode, string flightNumber,
        DateTimeOffset std, DateTimeOffset periodBegin, DateTimeOffset periodEnd, string destinationAirportCode)
        : base(scheduleSeasonCode, airlineCode, flightNumber)
    {
        Std = std;
        PeriodBegin = periodBegin;
        PeriodEnd = periodEnd;
        DestinationAirportCode = destinationAirportCode;
    }

    public static SeasonalFlightScheduleDepartureFlight Create(string scheduleSeasonCode, string airlineCode, string flightNumber,
        DateTimeOffset std, DateTimeOffset periodBegin, DateTimeOffset periodEnd, string destinationAirportCode)
        => new(scheduleSeasonCode, airlineCode, flightNumber, std, periodBegin, periodEnd, destinationAirportCode);

    public void UpdateStd(DateTimeOffset std, DateTimeOffset? ptd = null)
    {
        Std = std;
        Ptd = ptd;
    }

    public void UpdatePeriod(DateTimeOffset periodBegin, DateTimeOffset periodEnd)
    {
        PeriodBegin = periodBegin;
        PeriodEnd = periodEnd;
    }

    public void UpdateDestination(string destinationAirportCode) => DestinationAirportCode = destinationAirportCode;

    public void LinkArrivalFlight(int arrivalFlightId) => LinkedArrivalFlightId = arrivalFlightId;

    public void ReplaceRoutings(IEnumerable<(string AirportCode, int LegNumber, string DirectionCode, DateTimeOffset? Sta, DateTimeOffset? Std)> routings)
    {
        _routings.Clear();
        foreach (var r in routings)
        {
            var routing = SeasonalFlightScheduleDepartureFlightRouting.Create(Id, r.AirportCode, r.LegNumber, r.DirectionCode);
            routing.UpdateTimes(r.Sta, r.Std);
            _routings.Add(routing);
        }
    }
}
