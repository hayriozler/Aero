# Flight Outbound Event — Payload Examples

Implemented for `flight.arrival.created` / `flight.departure.created` / `flight.arrival.updated` /
`flight.departure.updated` on `aeroflight.flights`
(`AeroFlight.Infrastructure/Persistence/Interceptors/FlightOutboxInterceptor.cs`).

## Shape

- Two payload types, not one: `ArrivalFlightChanged` and `DepartureFlightChanged`
  (`AeroFlight.Contracts/Outbound/FlightChanged.cs`), both inheriting a shared `FlightChanged` base.
  `ArrivalFlight` and `DepartureFlight` are separate tables with **independent id sequences**, so the
  same numeric `flight.id` can legitimately belong to two unrelated flights — splitting the payload
  type keeps a consumer's `(type, id)` lookup unambiguous, and makes it structurally impossible for a
  departure-only resource kind to appear on an arrival payload (`CountersResources` only exists on
  `DepartureFlightChanged`).
- `flight` is always the **full current snapshot** (flat fields, no old/new wrapping) — consumers reading only
  current state never need to unwrap anything. Field names are the domain entity's own EF property names,
  camelCased — **not** the `ArrivalFlightDetail`/`DepartureFlightDetail` API response DTO names, which rename
  a few fields (e.g. entity `GateStatusCode` → DTO `GateStatus`). This keeps `changes` keys and `flight` keys
  in exact 1:1 correspondence by construction.
- `changes` is a map of only the fields that actually changed on this event, each as `{ "old": ..., "new": ... }`.
  On a `.created` event this is always `{}` — every field is new, so there's nothing to diff.
- Resource collections (`gatesResources`, `beltsResources`, `parkPositionsResources`, `routingsResources`, and
  — departure only — `countersResources`) carry their own per-item `operation` (`inserted` / `updated` /
  `deleted`) instead of appearing in `changes`. Resources assigned in the *same* SaveChanges call as a
  flight's own creation are included too, tagged `inserted`.
- `routingsResources`: the individual routing legs (`OpsArrivalRouting`/`OpsDepartureRouting` — `code`,
  `legNumber`, per-leg `sta`/`std`, etc.), not just the flattened `originAirportCode`/`routingSummary` on
  `flight`. `OpsRouting` isn't a `ResourceBase` subtype (different base, `FlightResourceBase`), so this is a
  separate collection rather than folding into gates/belts.
- `parkPositionsResources`: `OpsFlightParkPosition` rows can reference an arrival leg, a departure leg, or
  both (turnaround). A turnaround change is reported as two independent events — one for the arrival flight,
  one for the departure flight — never merged into one message, even though they share the same park-position
  row.
- No `tenantId` field in the payload — the outbox dispatcher already prefixes the routing key with the
  tenant id and sets it as a message header (same convention as the existing `ReferenceDataChanged` event).

## Scope / not implemented

- `flight.status.changed`, `flight.gate.assigned`, `flight.belt.assigned`, `flight.counter.assigned`,
  `flight.delay.added`, `flight.cancelled`, `flight.diverted` — the other granular routing keys already
  declared in `RoutingKeys.Outbound` are **not wired up**. Only the consolidated `*.created` / `*.updated`
  events fire; a cancellation, for example, just rides `*.updated` like any other field edit.

## 1) `flight.arrival.created` — new arrival with a gate and a park position assigned at creation

```json
{
  "eventId": "9b6e2a10-4c3d-4e91-9a2f-7d1b0c4e8a55",
  "eventType": "flight.arrival.created",
  "flightType": "Arrival",
  "occurredAt": "2026-08-29T08:00:00Z",
  "changedBy": "operator1",
  "flight": {
    "id": 481300,
    "airlineCode": "TK",
    "flightNumber": "TK 010",
    "flightDate": "2026-08-29",
    "status": "SCH",
    "terminalCode": "T2",
    "gates": "C7",
    "gateStatusCode": "A",
    "sta": "2026-08-29T14:00:00Z",
    "originAirportCode": "AMS",
    "createdAt": "2026-08-29T08:00:00Z",
    "createdBy": "operator1"
  },
  "changes": {},
  "gatesResources": [
    {
      "operation": "inserted",
      "resource": {
        "id": 991200, "flightId": 481300, "code": "C7", "lastCode": null,
        "beginPlan": "2026-08-29T13:45:00Z", "endPlan": "2026-08-29T14:30:00Z",
        "beginActual": null, "endActual": null, "displayCode": "C7", "statusCode": "A"
      }
    }
  ],
  "parkPositionsResources": [
    {
      "operation": "inserted",
      "resource": {
        "id": 992100, "arrivalFlightId": 481300, "departureFlightId": null, "code": "P12", "lastCode": null,
        "order": 1, "beginPlan": "2026-08-29T13:45:00Z", "endPlan": "2026-08-29T15:00:00Z",
        "beginActual": null, "endActual": null, "towCode": null, "towIndicator": null,
        "handlingAgentCode": null, "flightStatusCode": null
      }
    }
  ]
}
```

## 2) `flight.departure.updated` — status + counter change, no resource collections

```json
{
  "eventId": "b3f4a1e0-9c2d-4e77-8a11-2f6d0c9e5b71",
  "eventType": "flight.departure.updated",
  "flightType": "Departure",
  "occurredAt": "2026-08-28T12:05:30Z",
  "changedBy": "operator2",
  "flight": {
    "id": 481230,
    "airlineCode": "TK",
    "flightNumber": "TK 002",
    "flightDate": "2026-08-28",
    "status": "BRD",
    "terminalCode": "T2",
    "counters": "18-22",
    "counterStatusCode": "A",
    "std": "2026-08-28T13:00:00Z",
    "destinationAirportCode": "LHR",
    "updatedAt": "2026-08-28T12:05:30Z",
    "updatedBy": "operator2"
  },
  "changes": {
    "status": { "old": "SCH", "new": "BRD" },
    "counters": { "old": "15-17", "new": "18-22" }
  }
}
```

Note `countersResources`/`counters` only ever appear on a `DepartureFlightChanged` payload —
`ArrivalFlightChanged` has no such field, by construction (see Shape above).

## 3) `flight.arrival.updated` — cancellation

There is no separate `flight.status.changed` event (see Scope below) — a cancellation goes through
`UpdateArrivalFlightCommandHandler` like any other edit, so it publishes the same consolidated event;
`status` and `cancellationDateTime` just happen to be the only fields that changed.

```json
{
  "eventId": "1a9d7e60-4f3b-4c88-9e2a-6b0f7d3c8e12",
  "eventType": "flight.arrival.updated",
  "flightType": "Arrival",
  "occurredAt": "2026-08-28T09:15:00Z",
  "changedBy": "operator1",
  "flight": {
    "id": 481240,
    "airlineCode": "PC",
    "flightNumber": "PC 210",
    "flightDate": "2026-08-28",
    "status": "CNL",
    "cancellationDateTime": "2026-08-28T09:15:00Z",
    "updatedAt": "2026-08-28T09:15:00Z",
    "updatedBy": "operator1"
  },
  "changes": {
    "status": { "old": "SCH", "new": "CNL" },
    "cancellationDateTime": { "old": null, "new": "2026-08-28T09:15:00Z" }
  }
}
```

## 4) `flight.arrival.updated` — belt swap (resource collection) + weight change

```json
{
  "eventId": "e5c2a908-1d4f-4a6e-b7c3-9f8e0a1b2c33",
  "eventType": "flight.arrival.updated",
  "flightType": "Arrival",
  "occurredAt": "2026-08-28T11:50:00Z",
  "changedBy": "operator1",
  "flight": {
    "id": 481221,
    "airlineCode": "TK",
    "flightNumber": "TK 001",
    "flightDate": "2026-08-28",
    "status": "LAN",
    "belts": "B8",
    "beltStatusCode": "A",
    "baggageWeight": 3120.5,
    "baggageTotal": 198,
    "updatedAt": "2026-08-28T11:50:00Z",
    "updatedBy": "operator1"
  },
  "changes": {
    "belts": { "old": "B3", "new": "B8" },
    "baggageWeight": { "old": 2870.0, "new": 3120.5 }
  },
  "beltsResources": [
    {
      "operation": "deleted",
      "resource": {
        "id": 990101, "flightId": 481221, "code": "B3", "lastCode": null,
        "beginPlan": "2026-08-28T11:45:00Z", "endPlan": "2026-08-28T12:15:00Z",
        "beginActual": null, "endActual": null, "displayCode": "B3", "statusCode": "A"
      }
    },
    {
      "operation": "inserted",
      "resource": {
        "id": 990102, "flightId": 481221, "code": "B8", "lastCode": null,
        "beginPlan": "2026-08-28T11:45:00Z", "endPlan": "2026-08-28T12:15:00Z",
        "beginActual": null, "endActual": null, "displayCode": "B8", "statusCode": "A"
      }
    }
  ]
}
```

Note: `resource` carries every EF-tracked property of the resource entity generically (so `OpsArrivalBelt.LastBag`,
`OpsCounter.CheckinClassCode`, etc. show up automatically for those resource kinds) — the fields above are the
common `ResourceBase` ones shared by all of them. `parkPositionsResources` items carry `OpsFlightParkPosition`'s
own shape instead (`arrivalFlightId`/`departureFlightId`/`order`/`towCode`/etc. — see example 1), since it isn't
a `ResourceBase` subtype.

## Open question

`flight.gates` / `flight.belts` (summary string on the flight) and `gatesResources` / `beltsResources`
(time-windowed resource rows) carry overlapping information — that duplication comes from the existing
domain model, not from the event shape itself.
