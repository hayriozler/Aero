using AeroFlight.Contracts.Flights;

namespace AeroFlight.Application.Flights.Commands;

public enum CreateFlightOutcome { Created, DuplicateFlight }

public sealed record CreateFlightResult(CreateFlightOutcome Outcome, FlightResponse? Flight);
