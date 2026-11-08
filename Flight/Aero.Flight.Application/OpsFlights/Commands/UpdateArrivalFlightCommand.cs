using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.OpsFlights.Commands;

public sealed record UpdateArrivalFlightCommand(long Id, UpdateArrivalFlightRequest Request);
