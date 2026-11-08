using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.OpsFlights.Commands;

public sealed record UpdateDepartureFlightCommand(long Id, UpdateDepartureFlightRequest Request);
