using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.Flights.Commands;

public sealed record UpdateDepartureFlightCommand(long Id, UpdateDepartureFlightRequest Request);
