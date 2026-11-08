using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.OpsFlights.Commands;

public sealed record CreateDepartureFlightCommand(DepartureCreateRequest Request);
