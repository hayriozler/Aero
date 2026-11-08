using Aero.Flights.Contracts.SeasonalFlightSchedules;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Commands;

public sealed record UpdateScheduleSeasonCommand(string Code, ScheduleSeasonRequest Request);
