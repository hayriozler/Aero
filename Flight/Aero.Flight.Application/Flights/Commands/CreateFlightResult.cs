using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.Flights.Commands;

public enum CreateFlightOutcome { Created, DuplicateFlight }

public sealed record CreateFlightResult(CreateFlightOutcome Outcome, FlightResponse? Flight);
