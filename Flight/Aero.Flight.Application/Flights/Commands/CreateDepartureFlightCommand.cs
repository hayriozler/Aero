using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.Flights.Commands;

public sealed record CreateDepartureFlightCommand(DepartureCreateRequest Request);
