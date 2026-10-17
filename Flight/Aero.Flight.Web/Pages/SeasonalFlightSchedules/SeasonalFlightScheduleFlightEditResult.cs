using Aero.Flights.Contracts.SeasonalFlightSchedules;

namespace Aero.Flights.Web.Pages.SeasonalFlightSchedules;

public sealed record SeasonalFlightScheduleFlightEditResult(
    string Direction,
    SeasonalFlightScheduleArrivalFlightRequest? ArrivalRequest,
    SeasonalFlightScheduleDepartureFlightRequest? DepartureRequest);
