using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.OpsFlights.Commands;

public enum CreateFlightOutcome { Created, DuplicateFlight }

public sealed record CreateFlightResult(CreateFlightOutcome Outcome, FlightResponse? Flight);
