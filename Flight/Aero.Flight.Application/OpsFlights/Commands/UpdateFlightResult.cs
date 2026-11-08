using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.OpsFlights.Commands;

public enum UpdateFlightOutcome { Updated, NotFound, DuplicateFlight }

public sealed record UpdateFlightResult(UpdateFlightOutcome Outcome, FlightResponse? Flight);
