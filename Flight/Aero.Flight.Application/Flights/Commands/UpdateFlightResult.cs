using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.Flights.Commands;

public enum UpdateFlightOutcome { Updated, NotFound, DuplicateFlight }

public sealed record UpdateFlightResult(UpdateFlightOutcome Outcome, FlightResponse? Flight);
