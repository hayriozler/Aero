using AeroFlight.Contracts.SeasonalFlightSchedules;

namespace AeroFlight.Web.Pages.SeasonalFlightSchedules;

public sealed record SeasonalFlightScheduleFlightEditResult(
    string Direction,
    SeasonalFlightScheduleArrivalFlightRequest? ArrivalRequest,
    SeasonalFlightScheduleDepartureFlightRequest? DepartureRequest);
