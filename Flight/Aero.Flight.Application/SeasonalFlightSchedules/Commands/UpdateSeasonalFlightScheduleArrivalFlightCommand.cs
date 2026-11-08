using Aero.Flights.Contracts.SeasonalFlightSchedules;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Commands;

public sealed record UpdateSeasonalFlightScheduleArrivalFlightCommand(int Id, SeasonalFlightScheduleArrivalFlightRequest Request);
