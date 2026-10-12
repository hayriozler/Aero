namespace AeroFlight.Contracts.Inbound;

public sealed record CodeshareLeg(string AirlineCode, string FlightNumber);

// FlightTypeCode is required — Flight.FlightTypeCode is IsRequired() with a FK to FlightType
// (FlightConfiguration.cs), no sensible default exists, so it can't be optional here.
public sealed record ArrivalFlightCreated(
    string FlightNumber, string AirlineCode, DateTimeOffset Sta, string OriginAirportCode, string FlightTypeCode,
    string? Callsign = null, string? AircraftTypeCode = null,
    string? AircraftCode = null, string? OperatingAirlineCode = null, string? TerminalCode = null,
    string? StatusCode = null,
    int? PaxTotal = null, int? CodeshareNumber = null, IReadOnlyList<CodeshareLeg>? Codeshares = null);

public sealed record DepartureFlightCreated(
    string FlightNumber, string AirlineCode, DateTimeOffset Std, string DestinationAirportCode, string FlightTypeCode,
    string? Callsign = null, string? AircraftTypeCode = null,
    string? AircraftCode = null, string? OperatingAirlineCode = null, string? TerminalCode = null,
    string? StatusCode = null,
    int? PaxTotal = null, int? CodeshareNumber = null, IReadOnlyList<CodeshareLeg>? Codeshares = null);

// Deletes the matched flight — cascades to the linked turnaround flight per DeleteLinkedFlight,
// same behavior as DeleteArrivalFlightCommand/DeleteDepartureFlightCommand used by the API.
public sealed record FlightDeleted(string FlightNumber, DateTimeOffset Scheduled, long? Id = null, bool DeleteLinkedFlight = false);

// One generic message covers assign/update/release for every resource kind (Gate/Belt/Counter),
// rather than a dedicated message+processor pair per kind. ResourceKind is "Gate", "Belt", or
// "Counter" (Counter only valid on the departure routing key). Resources batches multiple
// operations for that one flight+kind under a single lookup and a single SaveChanges — e.g. a belt
// swap is one "release" + one "assign" in the same message, applied atomically. Each operation's own
// row is identified by ResourceId if set, else by (flight, ResourceKind, Code) among that flight's
// own resource rows of that kind — see FlightInboundProcessor.
public sealed record FlightResourceOperation(
    string Operation, string? Code = null, long? ResourceId = null,
    DateTimeOffset? BeginPlan = null, DateTimeOffset? EndPlan = null,
    DateTimeOffset? BeginActual = null, DateTimeOffset? EndActual = null,
    string? StatusCode = null);

public sealed record FlightResourceMessage(
    string FlightNumber, DateTimeOffset Scheduled, string ResourceKind,
    IReadOnlyList<FlightResourceOperation> Resources, long? Id = null);

// Consolidated update: any non-null field is applied; identifies the flight by id if set, else by number + scheduled time.
public sealed record ArrivalFlightUpdated(
    string FlightNumber, DateTimeOffset Sta, long? Id = null,
    DateTimeOffset? Eta = null, DateTimeOffset? Ata = null, DateTimeOffset? OnBlock = null,
    string? StatusCode = null,
    string? TerminalCode = null, string? ParkPositionCode = null,
    int? PaxTotal = null);

public sealed record DepartureFlightUpdated(
    string FlightNumber, DateTimeOffset Std, long? Id = null,
    DateTimeOffset? Etd = null, DateTimeOffset? Atd = null, DateTimeOffset? OffBlock = null,
    string? FlightStatusCode = null,
    string? TerminalCode = null, string? ParkPositionCode = null,
    int? PaxTotal = null);
