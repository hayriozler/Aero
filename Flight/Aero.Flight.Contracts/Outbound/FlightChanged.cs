using System.Text.Json;

namespace Aero.Flights.Contracts.Outbound;

public sealed record ChangedField(object? Old, object? New);

public sealed record ResourceChanged(string Operation, JsonElement Resource);

public abstract record FlightChanged(
    Guid EventId,
    string EventType,
    string FlightType,
    DateTimeOffset OccurredAt,
    string ChangedBy,
    string TimeZoneManager,
    JsonElement Flight,
    IReadOnlyDictionary<string, ChangedField> Changes,
    IReadOnlyList<ResourceChanged>? GatesResources = null,
    IReadOnlyList<ResourceChanged>? BeltsResources = null,
    IReadOnlyList<ResourceChanged>? ParkPositionsResources = null,
    IReadOnlyList<ResourceChanged>? RoutingsResources = null,
    IReadOnlyList<ResourceChanged>? PassengersResources = null);

public sealed record ArrivalFlightChanged(
    Guid EventId,
    string EventType,
    DateTimeOffset OccurredAt,
    string ChangedBy,
    string TimeZoneManager,
    JsonElement Flight,
    IReadOnlyDictionary<string, ChangedField> Changes,
    IReadOnlyList<ResourceChanged>? GatesResources = null,
    IReadOnlyList<ResourceChanged>? BeltsResources = null,
    IReadOnlyList<ResourceChanged>? ParkPositionsResources = null,
    IReadOnlyList<ResourceChanged>? RoutingsResources = null,
    IReadOnlyList<ResourceChanged>? PassengersResources = null)
    : FlightChanged(EventId, EventType, "Arrival", OccurredAt, ChangedBy, TimeZoneManager, Flight, Changes, GatesResources, BeltsResources, ParkPositionsResources, RoutingsResources, PassengersResources);

public sealed record DepartureFlightChanged(
    Guid EventId,
    string EventType,
    DateTimeOffset OccurredAt,
    string ChangedBy,
    string TimeZoneManager,
    JsonElement Flight,
    IReadOnlyDictionary<string, ChangedField> Changes,
    IReadOnlyList<ResourceChanged>? GatesResources = null,
    IReadOnlyList<ResourceChanged>? BeltsResources = null,
    IReadOnlyList<ResourceChanged>? ParkPositionsResources = null,
    IReadOnlyList<ResourceChanged>? RoutingsResources = null,
    IReadOnlyList<ResourceChanged>? CountersResources = null,
    IReadOnlyList<ResourceChanged>? PassengersResources = null)
    : FlightChanged(EventId, EventType, "Departure", OccurredAt, ChangedBy, TimeZoneManager, Flight, Changes, GatesResources, BeltsResources, ParkPositionsResources, RoutingsResources, PassengersResources);
