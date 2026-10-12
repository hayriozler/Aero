namespace AeroFlight.Contracts.Flights;

public sealed record UpdateFlightStatusRequest(string StatusCode);
public sealed record UpdateFlightTerminalRequest(string? TerminalCode);
public sealed record UpdateFlightRegistrationRequest(string? AircraftCode);
