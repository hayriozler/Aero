namespace Aero.Flights.Domain.SeasonalFlightSchedules;

public sealed class SeasonalFlightScheduleArrivalFlight : SeasonalFlightScheduleFlight
{
    private readonly List<SeasonalFlightScheduleArrivalFlightRouting> _routings = [];

    public DateTimeOffset Sta { get; private set; }
    public DateTimeOffset PeriodBegin { get; private set; }
    public DateTimeOffset PeriodEnd { get; private set; }
    public string OriginAirportCode { get; private set; } = string.Empty;
    public int? LinkedDepartureFlightId { get; private set; }

    public IReadOnlyCollection<SeasonalFlightScheduleArrivalFlightRouting> Routings => _routings;

    private SeasonalFlightScheduleArrivalFlight() { }

    private SeasonalFlightScheduleArrivalFlight(string scheduleSeasonCode, string airlineCode, string flightNumber,
        DateTimeOffset sta, DateTimeOffset periodBegin, DateTimeOffset periodEnd, string originAirportCode)
        : base(scheduleSeasonCode, airlineCode, flightNumber)
    {
        Sta = sta;
        PeriodBegin = periodBegin;
        PeriodEnd = periodEnd;
        OriginAirportCode = originAirportCode;
    }

    public static SeasonalFlightScheduleArrivalFlight Create(string scheduleSeasonCode, string airlineCode, string flightNumber,
        DateTimeOffset sta, DateTimeOffset periodBegin, DateTimeOffset periodEnd, string originAirportCode)
        => new(scheduleSeasonCode, airlineCode, flightNumber, sta, periodBegin, periodEnd, originAirportCode);

    public void UpdateSta(DateTimeOffset sta) => Sta = sta;

    public void UpdatePeriod(DateTimeOffset periodBegin, DateTimeOffset periodEnd)
    {
        PeriodBegin = periodBegin;
        PeriodEnd = periodEnd;
    }

    public void UpdateOrigin(string originAirportCode) => OriginAirportCode = originAirportCode;

    public void LinkDepartureFlight(int departureFlightId) => LinkedDepartureFlightId = departureFlightId;

    public void ReplaceRoutings(IEnumerable<(string AirportCode, int LegNumber, string DirectionCode, DateTimeOffset? Sta, DateTimeOffset? Std)> routings)
    {
        _routings.Clear();
        foreach (var r in routings)
        {
            var routing = SeasonalFlightScheduleArrivalFlightRouting.Create(Id, r.AirportCode, r.LegNumber, r.DirectionCode);
            routing.UpdateTimes(r.Sta, r.Std);
            _routings.Add(routing);
        }
    }
}
