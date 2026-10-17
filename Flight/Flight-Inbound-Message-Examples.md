# Flight Inbound Message — Payload Examples

Manual test payloads for the inbound flight messages consumed on `aeroflight.inbound`
(processed by `AeroFlight.Application/Flights/InboundProcessors/`). To publish one manually via the
RabbitMQ management UI: **Exchanges → aeroflight.inbound → Publish message**, set the routing key
shown under each example, add a `tenant` header (**required** — `InboxConsumerService` drops any
message without one), and paste the JSON body.

## Datetime convention: UTC, or local time with an explicit offset

Every `DateTimeOffset` field always carries its offset as part of the value — `"...Z"` (UTC) and
`"2026-08-30T14:30:00+03:00"` (local time, explicit offset) represent the exact same instant, and
.NET/Npgsql normalize to UTC automatically on parse/storage. No manual conversion is needed either
way, **as long as an explicit offset is present**. What's dangerous is a *bare* local time with no
offset at all (e.g. `"2026-08-30T14:30:00"`) — .NET's default parsing for that assumes the *server
machine's* local timezone, not the airport's, which would silently misinterpret the value. Always
include an explicit offset; the examples below use UTC (`...Z`) for clarity, but a correctly-offset
local time works identically.

One related fix this drove: the `FlightDate` fallback match in `FlightInboundProcessor.FindArrivalAsync`/
`FindDepartureAsync` extracts the calendar day via `sta.UtcDateTime.Date` (not the offset-relative
`.Date`) — so a message sent in local time with an explicit offset still resolves to the same UTC
calendar day the domain itself computed, even right around midnight.

## Id shortcut

Every message below except the two Create ones also accepts an optional `"id": <flightId>` — when
present it bypasses `flightNumber` + date matching entirely (`FindArrivalAsync`/`FindDepartureAsync`)
and looks the flight up directly by its AeroFlight-assigned id.

## One update routing key per leg — no per-field routing keys

There is no `flight.arrival.schedule.changed`/`.status.changed`/`.gate.updated` etc. — those were
removed. `flight.arrival.updated`/`flight.departure.updated` are the *only* update routing keys:
"any non-null field is applied" already tells the processor which field changed, so a single-field
change (ETA only, status only, gate only, ...) and a multi-field MVT-style update both go through the
same message shape, just with fewer or more fields populated. See examples 3–4.

## Known bug: gate summary field

`ArrivalFlightUpdated.Gates`/`DepartureFlightUpdated.Gates` currently write the incoming gate code
into the 1-character `GateStatusCode` column instead of the `Gates` field (wrong method overload
called in `FlightMessageMapper.ApplyUpdate`) — it will throw on save for any code longer than 1
character. Not yet fixed as of this writing.

---

## 1) Create — arrival

`flight.arrival.created` — `flightTypeCode` is required (`Flight.FlightTypeCode` is `IsRequired()`
with a FK to `FlightType`; must be an existing code in that reference table, e.g. `"J"` for scheduled
passenger)

```json
{
  "flightNumber": "TK 001",
  "airlineCode": "TK",
  "sta": "2026-08-30T11:30:00Z",
  "originAirportCode": "JFK",
  "flightTypeCode": "J",
  "terminalCode": "T2",
  "statusCode": "SCH",
  "paxTotal": 210
}
```

## 2) Create — departure

`flight.departure.created` — `flightTypeCode` required, same as above

```json
{
  "flightNumber": "TK 002",
  "airlineCode": "TK",
  "std": "2026-08-30T13:00:00Z",
  "destinationAirportCode": "LHR",
  "flightTypeCode": "J",
  "terminalCode": "T2",  
  "statusCode": "SCH"
}
```

## 3) Update — arrival, single field only (ETA)

`flight.arrival.updated` — omitting every other field leaves them untouched

```json
{
  "flightNumber": "TK 001",
  "sta": "2026-08-30T11:30:00Z",
  "eta": "2026-08-30T11:50:00Z"
}
```

## 4) Update — arrival, multiple fields at once (MVT-style)

`flight.arrival.updated`

```json
{
  "flightNumber": "TK 001",
  "sta": "2026-08-30T11:30:00Z",
  "eta": "2026-08-30T11:45:00Z",
  "ata": "2026-08-30T11:42:00Z",
  "statusCode": "LAN",
  "belts": "B3",
  "terminalCode": "T2",
  "parkPositionCode": "P12"
}
```

## 5) Update — departure, multiple fields at once

`flight.departure.updated` — note the field is `flightStatusCode`, not `statusCode`; note `std`, not `scheduled`

```json
{
  "flightNumber": "TK 002",
  "std": "2026-08-30T13:00:00Z",
  "etd": "2026-08-30T13:15:00Z",
  "flightStatusCode": "BRD",
  "counters": "18-22",
  "terminalCode": "T2"
}
```

## 6) Resource — assign a gate (arrival)

`flight.arrival.resource.changed` — `FlightResourceMessage`, one generic message for assign/update/
release across every resource kind (dispatched by `resourceKind`, not a message-per-kind), batching
one or more operations under `resources` per flight+kind

```json
{
  "flightNumber": "TK 001",
  "scheduled": "2026-08-30T11:30:00Z",
  "resourceKind": "Gate",
  "resources": [
    {
      "operation": "assign",
      "code": "C4",
      "beginPlan": "2026-08-30T11:15:00Z",
      "endPlan": "2026-08-30T12:00:00Z",
      "statusCode": "A"
    }
  ]
}
```
`resourceKind` is `"Gate"` or `"Belt"` on the arrival routing key. `code` may be omitted at assign
time — a code-less placeholder resource row is a valid state (assigned later via the Resources tab).

## 7) Resource — belt swap in one message (departure): release old + assign new

`flight.departure.resource.changed` — every operation in `resources` shares one flight lookup and one
`SaveChanges`, so a swap is atomic, not two separate round trips

```json
{
  "flightNumber": "TK 002",
  "std": "2026-08-30T13:00:00Z",
  "resourceKind": "Belt",
  "resources": [
    { "operation": "release", "code": "B3" },
    { "operation": "assign", "code": "B8", "beginPlan": "2026-08-30T12:45:00Z", "endPlan": "2026-08-30T13:15:00Z" }
  ]
}
```
Each item without a `resourceId` is matched by `(flight, resourceKind, code)` among that flight's
existing rows of that kind — here `release` matches the existing `B3` row by code.

## 8) Resource — release a counter by resourceId (departure only)

`flight.departure.resource.changed` — `resourceKind: "Counter"` is only valid on the departure key

```json
{
  "flightNumber": "TK 002",
  "std": "2026-08-30T13:00:00Z",
  "resourceKind": "Counter",
  "resources": [
    { "operation": "release", "resourceId": 990310 }
  ]
}
```
`resourceId`, when present on an operation, always takes priority over the `(flight, resourceKind,
code)` fallback — same id-preferred pattern used for the flight lookup itself.

## 9) Delete — arrival

`flight.arrival.deleted`

```json
{
  "flightNumber": "TK 001",
  "scheduled": "2026-08-30T11:30:00Z",
  "deleteLinkedFlight": false
}
```

`flight.departure.deleted` is the same shape (`FlightDeleted`). `deleteLinkedFlight: true` cascades
the delete to the linked turnaround flight — same behavior as the API's delete endpoint.
