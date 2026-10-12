using AeroFlight.Contracts.Flights;

namespace AeroFlight.Application.Flights.Commands;

public enum UpdateFlightOutcome { Updated, NotFound, DuplicateFlight }

public sealed record UpdateFlightResult(UpdateFlightOutcome Outcome, FlightResponse? Flight);
