# ARCHITECTURE.md — Baggage Reconciliation System (BRS)

> Authoritative system design document. Update this file alongside code changes via ADR.

---

## Table of Contents

- [1. System Context](#1-system-context)
- [2. Application Architecture](#2-application-architecture)
- [3. Module Breakdown](#3-module-breakdown)
- [4. Domain Model](#4-domain-model)
- [5. IATA Message Processing](#5-iata-message-processing)
- [6. HHT Scanner Architecture](#6-hht-scanner-architecture)
- [7. Baggage Office Web UI](#7-baggage-office-web-ui)
- [8. Data Architecture](#8-data-architecture)
- [9. Security Architecture](#9-security-architecture)
- [10. Observability](#10-observability)
- [11. Deployment Architecture](#11-deployment-architecture)
- [12. Failure Modes & Resilience](#12-failure-modes--resilience)
- [13. Performance Targets](#13-performance-targets)
- [14. ADR Index](#14-adr-index)

---

## 1. System Context

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          AIRPORT ECOSYSTEM                              │
│                                                                         │
│  ┌─────────┐   SFTP/TCP    ┌──────────┐    SITA/ARINC   ┌──────────┐  │
│  │  AODB   │──────────────▶│  BRS     │◀───────────────▶│ Airline  │  │
│  │(Amadeus │              │  SYSTEM  │   BSM/BPM/BTM   │  DCS     │  │
│  │ Altéa / │              │          │                  │(Type B)  │  │
│  │ AIMS)   │◀─────────────│          │                  └──────────┘  │
│  └─────────┘   REST/WS    │          │                                 │
│                            │          │   ┌──────────────────────────┐ │
│  ┌─────────┐               │          │──▶│   Baggage Office Staff   │ │
│  │  BHS    │◀──────────────│          │   │   (Blazor Web App)       │ │
│  │(Baggage │   Scan cmds   │          │   └──────────────────────────┘ │
│  │Handling │──────────────▶│          │                                 │
│  │System)  │   Sort events │          │   ┌──────────────────────────┐ │
│  └─────────┘               │          │──▶│  Ramp Agents / Loaders   │ │
│                            │          │   │  (.NET MAUI HHT Android) │ │
│  ┌─────────┐               │          │   └──────────────────────────┘ │
│  │ Customs │◀──────────────│          │                                 │
│  │  CUSRES │   XML/REST    │          │   ┌──────────────────────────┐ │
│  └─────────┘               │          │──▶│   Airport Management     │ │
│                            │          │   │   Dashboard (Blazor)     │ │
│  ┌─────────┐               │          │   └──────────────────────────┘ │
│  │ Sortation│◀─────────────│          │                                 │
│  │ System  │  BSO messages └──────────┘                                │
│  └─────────┘                                                            │
└─────────────────────────────────────────────────────────────────────────┘
```

### External Systems

| System | Protocol | Direction | Purpose |
|---|---|---|---|
| AODB (Amadeus Altéa / AIMS) | REST/WebSocket | Bidirectional | Flight schedule, passenger lists, gate assignments |
| Airline DCS (Departure Control) | IATA Type B / EDIFACT | Inbound | BSM, BPM, BTM messages |
| BHS (Baggage Handling System) | TCP/SITA | Bidirectional | Sort commands, carousel assignments |
| Customs (CUSRES) | EDIFACT / REST | Outbound | Customs reconciliation messages |
| SITA WorldTracer | REST/SOAP | Bidirectional | Lost/found bag tracking |

---

## 2. Application Architecture

BRS is a **monolithic** ASP.NET Core 10 application. All business logic, background workers, and API endpoints live in a single deployable unit. Internal boundaries are enforced via module folders and dependency rules — not network calls.

```
                    ┌─────────────────────────────┐
                    │      Internet / Intranet    │
                    └──────────────┬──────────────┘
                                   │ HTTPS
                    ┌──────────────▼──────────────┐
                    │        BRS Monolith          │
                    │     ASP.NET Core 10          │
                    │                              │
                    │  ┌────────────────────────┐  │
                    │  │   Minimal API Layer    │  │
                    │  │  /api/v1/bags          │  │
                    │  │  /api/v1/flights       │  │
                    │  │  /api/v1/scans         │  │
                    │  │  /api/v1/reports       │  │
                    │  └───────────┬────────────┘  │
                    │              │                │
                    │  ┌───────────▼────────────┐  │
                    │  │   Application Modules  │  │
                    │  │  Baggage · Flights     │  │
                    │  │  Scanning · Reporting  │  │
                    │  │  Messaging · Audit     │  │
                    │  └───────────┬────────────┘  │
                    │              │                │
                    │  ┌───────────▼────────────┐  │
                    │  │   Background Workers   │  │
                    │  │  AODB Poller           │  │
                    │  │  IATA Message Listener │  │
                    │  │  Reconciliation Runner │  │
                    │  └───────────┬────────────┘  │
                    │              │                │
                    │  ┌───────────▼────────────┐  │
                    │  │    Blazor 10 Web App   │  │
                    │  │  Server (dashboards)   │  │
                    │  │  WASM (reports/tools)  │  │
                    │  └────────────────────────┘  │
                    └──────────────┬───────────────┘
                                   │
                    ┌──────────────▼───────────────┐
                    │        PostgreSQL 16          │
                    │   Single database, logical   │
                    │   schema separation per      │
                    │   domain area                │
                    └──────────────────────────────┘

                    ┌─────────────────────────────────────────────────────┐
                    │          .NET MAUI HHT App (Android)                │
                    │   Offline-first · SQLite local · BLE scanner support│
                    └─────────────────────────────────────────────────────┘
```

---

## 3. Module Breakdown

The monolith is organized into vertical modules. Each module owns its own EF Core `DbContext`, endpoint registration, and application services. Modules communicate through direct method calls or in-process domain events — never via HTTP.

### 3.1 Baggage Module

**Responsibilities**:
- CRUD for `Bag` aggregate
- Bag lifecycle state machine (Created → CheckedIn → Sorted → Loaded → Arrived | Offloaded | Lost)
- Reconciliation calculation per flight
- Processing incoming BSM/BPM/BTM messages
- Raising in-process domain events (`BagStatusChanged`, `ReconciliationCompleted`)

**Endpoints**:
```
GET    /api/v1/bags/{tagNumber}              — Get bag by IATA tag
GET    /api/v1/bags?flightKey={key}          — List bags for a flight
POST   /api/v1/bags/checkin                  — Manual check-in (fallback)
PUT    /api/v1/bags/{tagNumber}/status       — Override bag status (supervisor)
GET    /api/v1/reconciliation/{flightKey}    — Full reconciliation report
POST   /api/v1/reconciliation/{flightKey}/close  — Close flight reconciliation
GET    /api/v1/bags/{tagNumber}/journey      — Full scan history for a bag
POST   /api/v1/bags/{tagNumber}/offload      — Mark bag as offloaded
POST   /api/v1/bags/transfer                 — Initiate bag transfer (BTM)
GET    /api/v1/bags/unmatched?flightKey={k}  — Bags on aircraft, no passenger
GET    /api/v1/bags/rush                     — Rush/priority bags across flights
```

**State Machine**:
```
              ┌──────────────┐
              │   CREATED    │◀── BSM received
              └──────┬───────┘
                     │ BSM confirmed + passenger matched
              ┌──────▼───────┐
              │  CHECKED_IN  │◀── Check-in desk scan
              └──────┬───────┘
                     │ Sortation scan
              ┌──────▼───────┐
              │    SORTED    │◀── BPM received / carousel assigned
              └──────┬───────┘
                     │ Loading scan
           ┌─────────┼─────────┐
    ┌───────▼──┐     │    ┌────▼──────┐
    │  LOADED  │     │    │ OFFLOADED │◀── Passenger offload / security
    └───────┬──┘     │    └───────────┘
            │        │ Transfer
    ┌───────▼──┐  ┌──▼─────────┐
    │ ARRIVED  │  │ TRANSFERRED │◀── BTM received
    └──────────┘  └────────────┘
```

---

### 3.2 Flight Module

**Responsibilities**:
- Sync flight data from AODB (polled via background worker)
- Maintain `Flight`, `FlightLeg`, `Passenger` entities
- Passenger list management (PNL/ADL processing)
- Gate and carousel assignment

**Endpoints**:
```
GET    /api/v1/flights                         — List flights (with filters)
GET    /api/v1/flights/{id}                    — Flight detail
GET    /api/v1/flights/{id}/passengers         — Passenger manifest
GET    /api/v1/flights/{id}/baggage-stats      — Bags checked vs loaded
```

---

### 3.3 Scanning Module

**Responsibilities**:
- Receive scan events from HHT devices (REST + WebSocket)
- Validate scan context (bag exists, flight active, location valid)
- Command HHT devices (screen messages, audio alerts, status updates)
- Buffer and order scan events (handle late-arriving offline scans)

**Endpoints**:
```
POST   /api/v1/scans                          — Submit scan event (online HHT)
POST   /api/v1/scans/batch                    — Submit offline scan queue
GET    /api/v1/scans/{flightKey}              — All scans for flight
GET    /api/v1/scans/device/{deviceId}/status — HHT device health
WS     /ws/scanner/{deviceId}                 — Real-time command channel
```

**Scan Event Schema**:
```json
{
  "deviceId": "HHT-042",
  "tagNumber": "0614123456789",
  "location": "GATE-B22",
  "scanType": "Loading",
  "flightKey": "XX1234/20250401/LHR",
  "deviceTimestamp": "2025-04-01T10:22:31.000Z",
  "latitude": 51.4775,
  "longitude": -0.4614,
  "operatorId": "OP-1098",
  "isOffline": false
}
```

---

### 3.4 Reporting Module

**Responsibilities**:
- Pre-aggregated reports (end-of-day, per airline, per terminal)
- Flight close reconciliation PDF generation (QuestPDF)
- CUSRES message generation and dispatch
- WorldTracer missing bag reporting
- Audit log export (CSV/Excel)

**Endpoints**:
```
GET    /api/v1/reports/flight/{flightKey}     — Flight reconciliation report
GET    /api/v1/reports/daily?date={d}         — Daily stats
GET    /api/v1/reports/airline/{code}?date={d}— Per-airline report
GET    /api/v1/reports/mishandled?date={d}    — Mishandled bags
POST   /api/v1/reports/cusres/{flightKey}     — Generate + send CUSRES
GET    /api/v1/audit?bagTag={t}               — Bag audit trail
GET    /api/v1/audit/export?from={d}&to={d}  — Bulk audit export
```

---

### 3.5 Background Workers

All workers run inside the monolith as `IHostedService` / `BackgroundService` implementations.

| Worker | Responsibility |
|---|---|
| `AodbPollerWorker` | Polls AODB for flight/passenger updates on a configurable interval |
| `IataListenerWorker` | Listens on TCP for incoming Type B / EDIFACT messages (BSM, BPM, BTM) |
| `ReconciliationWorker` | Runs reconciliation calculations for departing/arrived flights |
| `AuditWriterWorker` | Drains the in-memory audit queue and persists to `audit_log` |

---

## 4. Domain Model

### Core Aggregates

```csharp
// ── Bag Aggregate ────────────────────────────────────────────────────────
public sealed class Bag : AggregateRoot
{
    public BagTagNumber TagNumber { get; private set; }     // Value Object
    public BagStatus Status { get; private set; }           // Enum
    public FlightKey FlightKey { get; private set; }        // Value Object
    public PassengerId PassengerId { get; private set; }
    public Weight Weight { get; private set; }              // Value Object (kg)
    public BaggageClass Class { get; private set; }         // Economy/Business/First
    public IReadOnlyList<ScanEvent> ScanHistory { get; }
    public IReadOnlyList<SpecialHandling> SpecialHandlings { get; }
    public bool IsTransfer { get; private set; }
    public FlightKey? InboundFlightKey { get; private set; }

    // Domain methods
    public Result CheckIn(OperatorId operatedBy, DateTime timestamp) { }
    public Result MarkLoaded(Location location, OperatorId operatedBy) { }
    public Result Offload(OffloadReason reason, OperatorId operatedBy) { }
    public Result Transfer(FlightKey toFlight) { }
}

// ── Flight Aggregate ─────────────────────────────────────────────────────
public sealed class Flight : AggregateRoot
{
    public FlightKey Key { get; private set; }
    public string AirlineCode { get; private set; }
    public string FlightNumber { get; private set; }
    public DateOnly OperationalDate { get; private set; }
    public Airport Origin { get; private set; }
    public Airport Destination { get; private set; }
    public DateTime? ScheduledDeparture { get; private set; }
    public DateTime? EstimatedDeparture { get; private set; }
    public DateTime? ActualDeparture { get; private set; }
    public FlightStatus Status { get; private set; }
    public Gate? DepartureGate { get; private set; }
    public Carousel? ArrivalCarousel { get; private set; }
    public int TotalPassengers { get; private set; }
    public int TotalBagsChecked { get; private set; }
}

// ── ScanEvent (Entity within Bag) ───────────────────────────────────────
public sealed class ScanEvent : Entity
{
    public BagTagNumber TagNumber { get; }
    public ScanType Type { get; }            // CheckIn, Sortation, Loading, Arrival, Transfer
    public Location Location { get; }
    public DeviceId DeviceId { get; }
    public OperatorId OperatorId { get; }
    public DateTime DeviceTimestamp { get; }
    public DateTime ServerTimestamp { get; }
    public GeoCoordinate? Coordinate { get; }
}

// ── ReconciliationRecord (Aggregate) ─────────────────────────────────────
public sealed class ReconciliationRecord : AggregateRoot
{
    public FlightKey FlightKey { get; private set; }
    public ReconciliationStatus Status { get; private set; }
    public int TotalChecked { get; private set; }
    public int TotalLoaded { get; private set; }
    public int TotalOffloaded { get; private set; }
    public int TotalTransferred { get; private set; }
    public IReadOnlyList<BagTagNumber> UnmatchedBags { get; }  // On aircraft, no scan
    public IReadOnlyList<BagTagNumber> RushBags { get; }
    public DateTime? ClosedAt { get; private set; }
    public OperatorId? ClosedBy { get; private set; }
}
```

### Value Objects

```csharp
// 10-digit IATA tag: 3-char airline + 6-digit serial + 1 check digit
public readonly record struct BagTagNumber
{
    public string Value { get; }

    public static bool TryParse(string input, out BagTagNumber result) { }
    public static BagTagNumber Parse(string input) { }

    public string AirlineCode => Value[..3];
    public string SerialNumber => Value[3..9];
    public char CheckDigit => Value[9];
    public bool IsValid => LuhnValidate(Value);
}

// FlightNumber + OriginDate + DepartureStation
public readonly record struct FlightKey
{
    public string FlightNumber { get; }
    public string IataDate { get; }
    public string AirlineCode { get; }
    public string DepartureAirportCode { get; }

    public override string ToString() =>
        $"{AirlineCode}{FlightNumber}/{IataDate}/{DepartureAirportCode}";
}

public readonly record struct Weight(decimal Kilograms)
{
    public static Weight FromPounds(decimal lbs) => new(lbs * 0.453592m);
}
```

---

## 5. IATA Message Processing

IATA Type B / EDIFACT messages arrive over TCP and are processed synchronously by the `IataListenerWorker`. Failed parses are written to `raw_messages` with `parse_status = FAILED` for manual review.

### Message Sequence Handling

BSM → BPM → BTM can arrive out of order. The Baggage module uses **upsert semantics**:

```
On BSM receipt:
  → UPSERT Bag (if exists: update weight/passenger; if not: create)

On BPM receipt:
  → If Bag exists: update sortation/carousel data
  → If Bag NOT exists: create stub Bag in SORTED state, await BSM

On BTM receipt:
  → If source Bag exists: initiate transfer workflow
  → Create target Bag record with TRANSFERRED_IN status
  → Link inbound/outbound flight legs
```

---

## 6. HHT Scanner Architecture

### 6.1 Overview

The HHT (Hand Held Terminal) is a ruggedized Android device running .NET MAUI 10. It communicates with BRS via REST (online) or buffers locally (offline). Devices connect over airport Wi-Fi or 4G LTE.

```
┌────────────────────────────────────────────────────────────┐
│                 .NET MAUI HHT App (Android)                 │
│                                                            │
│  ┌──────────────┐    ┌───────────────┐   ┌─────────────┐  │
│  │  Scan Screen │    │ Flight Select │   │ Status Dash │  │
│  │  (Main UI)   │    │   Screen      │   │   Screen    │  │
│  └──────┬───────┘    └───────┬───────┘   └──────┬──────┘  │
│         │                    │                   │         │
│  ┌──────▼────────────────────▼───────────────────▼──────┐  │
│  │              ViewModel Layer (MVVM)                   │  │
│  │   ScanViewModel  FlightViewModel  StatusViewModel     │  │
│  └──────────────────────────┬────────────────────────────┘  │
│                             │                              │
│  ┌──────────────────────────▼────────────────────────────┐  │
│  │              Services Layer                            │  │
│  │  ScanService · FlightService · AuthService            │  │
│  │  ConnectivityService · SyncService                    │  │
│  └──────────┬─────────────────────┬──────────────────────┘  │
│             │                     │                        │
│  ┌──────────▼──────┐   ┌──────────▼──────────────────────┐  │
│  │  SQLite (local) │   │  HTTP Client → BRS API          │  │
│  │  Scan queue     │   │  (with retry & offline buffer)  │  │
│  │  Flight cache   │   └─────────────────────────────────┘  │
│  └─────────────────┘                                       │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Hardware Abstraction                                  │  │
│  │  BarcodeScanner (built-in HW) · Camera Fallback       │  │
│  │  Vibration · Audio · Screen backlight                 │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
```

### 6.2 MVVM Architecture

```csharp
// ScanViewModel.cs
[ObservableObject]
public partial class ScanViewModel
{
    private readonly IScanService _scanService;
    private readonly IConnectivityService _connectivity;

    [ObservableProperty]
    private string _lastTagNumber = string.Empty;

    [ObservableProperty]
    private ScanResultState _scanResult = ScanResultState.Idle;

    [ObservableProperty]
    private BagInfoDto? _lastBagInfo;

    [ObservableProperty]
    private bool _isOfflineMode;

    [RelayCommand]
    private async Task ProcessScanAsync(string tagNumber)
    {
        var scan = new ScanEventDto(
            DeviceId: DeviceId.Current,
            TagNumber: tagNumber,
            Location: CurrentLocation,
            ScanType: ScanType.Loading,
            FlightKey: SelectedFlightKey,
            DeviceTimestamp: DateTime.UtcNow,
            OperatorId: AuthState.CurrentOperator,
            IsOffline: !_connectivity.IsConnected
        );

        if (_connectivity.IsConnected)
        {
            var result = await _scanService.SubmitScanAsync(scan);
            HandleScanResponse(result);
        }
        else
        {
            await _scanService.EnqueueOfflineScanAsync(scan);
            ScanResult = ScanResultState.QueuedOffline;
            Vibrate(VibratePattern.Short);
        }
    }
}
```

### 6.3 Scan Response Feedback

| Result | Visual | Audio | Vibration |
|---|---|---|---|
| OK - Load | Green ✓ | Single beep | Short pulse |
| OK - Do Not Load | Red ✗ | Triple beep | Long pulse |
| Bag Not Found | Yellow ? | Double beep | Short-short |
| Already Loaded | Orange ⚠ | Double beep | Short-long |
| Wrong Flight | Red ✗ | Triple beep | Long-long |
| Offline Queued | Blue ⏳ | Single soft | Short |
| Server Error | Orange ! | Double beep | Short-long |

---

## 7. Baggage Office Web UI

### 7.1 Blazor Architecture

```
WebApp (Blazor 10) — hosted within the monolith
├── Server Interactive Mode — Dashboard, Reconciliation, Alerts
│   └── SignalR connection → in-process hubs
│
├── WASM Interactive Mode — Reports, Configuration, Audit
│   └── Loaded on demand (lazy) to keep initial payload small
│
└── Static SSR — Login, Landing, Error pages
```

**Component Hierarchy**:
```
App.razor
└── MainLayout.razor
    ├── NavMenu.razor (sidebar)
    ├── AlertBar.razor (top alert strip, SignalR-driven)
    └── [Page Components]
        ├── FlightDashboard.razor
        │   ├── FlightList.razor
        │   │   └── FlightRow.razor (live status updates)
        │   └── FlightDetail.razor
        │       ├── BagManifest.razor
        │       ├── ReconciliationPanel.razor
        │       └── ScanTimeline.razor
        ├── BagSearch.razor
        │   └── BagJourneyMap.razor
        ├── ReconciliationReport.razor (WASM)
        ├── AlertCenter.razor
        ├── HhtDeviceManager.razor
        └── Configuration/
            ├── CarouselConfig.razor
            ├── UserManagement.razor
            └── AirlineConfig.razor
```

### 7.2 Key Pages & Features

| Page | Mode | Key Features |
|---|---|---|
| Flight Dashboard | Server | Live flight list, countdown to departure, bag stats ticker |
| Bag Search | Server | Tag scan or manual search, full journey timeline |
| Reconciliation | Server | Live loaded/checked counts, unmatched bag list, close button |
| Alert Center | Server | Priority alerts, DO NOT LOAD commands, rush bags |
| Reports | WASM | PDF generation, date-range query, export |
| Audit Log | WASM | Full event log with filter, CSV export |
| HHT Manager | Server | Device health, operator list, force sync |
| Configuration | WASM | Airlines, airports, carousels, user roles |

---

## 8. Data Architecture

### 8.1 Single Database, Logical Schema Separation

All modules share one PostgreSQL database. Schemas provide logical isolation without the overhead of cross-service network calls.

| Schema | Module Owner | Key Tables |
|---|---|---|
| `brs_baggage` | Baggage Module | `bags`, `scan_events`, `bag_journeys`, `reconciliation_records`, `special_handlings` |
| `brs_flight` | Flight Module | `flights`, `flight_legs`, `passengers`, `carousels`, `gates` |
| `brs_messaging` | IATA Worker | `raw_messages`, `parse_errors` |
| `brs_reporting` | Reporting Module | `daily_stats`, `flight_reports` (read model) |
| `brs_audit` | Audit Module | `audit_log` (append-only, partitioned by month) |
| `brs_identity` | Auth | `users`, `roles`, `user_roles` (custom names — not ASP.NET Identity defaults) |

### 8.2 Key Table Definitions

```sql
-- bags
CREATE TABLE brs_baggage.bags (
    id              BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    tag_number      CHAR(10) NOT NULL,
    status          VARCHAR(20) NOT NULL,
    flight_key      VARCHAR(30) NOT NULL,
    passenger_id    BIGINT,
    weight_kg       NUMERIC(6,2),
    bag_class       VARCHAR(10),
    is_transfer     BOOLEAN NOT NULL DEFAULT FALSE,
    inbound_flight  VARCHAR(30),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    version         INTEGER NOT NULL DEFAULT 1,    -- Optimistic concurrency
    CONSTRAINT uq_tag_flight UNIQUE (tag_number, flight_key)
);
CREATE INDEX idx_bags_flight_key ON brs_baggage.bags (flight_key);
CREATE INDEX idx_bags_status ON brs_baggage.bags (status);
CREATE INDEX idx_bags_tag_number ON brs_baggage.bags (tag_number);

-- scan_events
CREATE TABLE brs_baggage.scan_events (
    id                  BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    bag_id              BIGINT NOT NULL REFERENCES brs_baggage.bags(id),
    tag_number          CHAR(10) NOT NULL,
    scan_type           VARCHAR(20) NOT NULL,
    location_code       VARCHAR(20) NOT NULL,
    device_id           VARCHAR(20),
    operator_id         VARCHAR(20),
    device_timestamp    TIMESTAMPTZ NOT NULL,
    server_timestamp    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    latitude            NUMERIC(10,7),
    longitude           NUMERIC(10,7),
    is_offline_sync     BOOLEAN NOT NULL DEFAULT FALSE
) PARTITION BY RANGE (server_timestamp);

-- raw_messages (IATA worker) — append-only
CREATE TABLE brs_messaging.raw_messages (
    id              BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    message_type    VARCHAR(10) NOT NULL,  -- BSM, BPM, BTM, CUSRES
    airline_code    CHAR(2),
    raw_payload     TEXT NOT NULL,
    received_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    parse_status    VARCHAR(10) NOT NULL,  -- PENDING, SUCCESS, FAILED
    parsed_at       TIMESTAMPTZ,
    error_details   TEXT
) PARTITION BY RANGE (received_at);

-- audit_log — immutable, append-only
CREATE TABLE brs_audit.audit_log (
    id              BIGINT GENERATED ALWAYS AS IDENTITY,
    entity_type     VARCHAR(30) NOT NULL,
    entity_id       BIGINT NOT NULL,
    action          VARCHAR(30) NOT NULL,
    actor_id        VARCHAR(50),
    actor_role      VARCHAR(30),
    before_state    JSONB,
    after_state     JSONB,
    metadata        JSONB,
    occurred_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
) PARTITION BY RANGE (occurred_at);
```

---

## 9. Security Architecture

### 9.1 Authentication & Authorization

JWT-based auth is implemented directly in ASP.NET Core using custom tables (no ASP.NET Identity defaults).

```
User / Device
     │
     │ Username + Password → JWT (Web / M2M)
     │ Device Flow (HHT)
     ▼
┌────────────────────────────────────┐
│       BRS Identity Module          │
│                                    │
│  Roles:                            │
│  - brs:operator          (HHT)     │
│  - brs:supervisor        (Web)     │
│  - brs:baggage-agent     (Web)     │
│  - brs:manager           (Web)     │
│  - brs:system            (M2M)     │
└────────────────────────────────────┘
```

### 9.2 Authorization Policies

```csharp
// Infrastructure/Auth/Policies.cs
public static class BrsPolicies
{
    public const string CanScanBags         = "CanScanBags";          // operator+
    public const string CanOffloadBag       = "CanOffloadBag";        // supervisor+
    public const string CanCloseFlight      = "CanCloseFlight";       // supervisor+
    public const string CanViewReports      = "CanViewReports";       // baggage-agent+
    public const string CanExportAudit      = "CanExportAudit";       // manager
    public const string CanManageUsers      = "CanManageUsers";       // manager
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(BrsPolicies.CanOffloadBag, policy =>
        policy.RequireRole("brs:supervisor", "brs:manager"));

    options.AddPolicy(BrsPolicies.CanCloseFlight, policy =>
        policy.RequireRole("brs:supervisor", "brs:manager"));

    options.AddPolicy(BrsPolicies.CanExportAudit, policy =>
        policy.RequireRole("brs:manager"));
});
```

### 9.3 Data Security

- **Bag tag numbers** in logs: redacted at INFO level and above, visible at DEBUG only
- **PII (passenger names, PNRs)**: encrypted at rest using PostgreSQL `pgcrypto`; column-level encryption for `passenger_name` and `booking_reference`
- **Secrets**: environment variables / `.env` in dev; secrets manager in prod
- **Audit**: every write operation writes to `brs_audit.audit_log` (immutable) via EF Core interceptor

---

## 10. Observability

### 10.1 OpenTelemetry Stack

```
BRS Monolith (OTel SDK)
      │
      │ OTLP (gRPC :4317)
      ▼
┌─────────────────────┐
│  OTel Collector     │
└──┬──────────┬───────┘
   │          │
   ▼          ▼
┌──────┐  ┌─────────┐
│Tempo │  │  Loki   │
│(traces)│ │(logs)  │
└──┬───┘  └────┬────┘
   │            │
   ▼            ▼
┌───────────────────────┐
│      Grafana          │
│  Traces · Logs        │
│  Dashboards · Alerts  │
└───────────────────────┘

Metrics → Prometheus → Grafana
```

### 10.2 Key Metrics

| Metric | Type | Alert Threshold |
|---|---|---|
| `scans.processed_total` | Counter | — |
| `scans.processing_duration` | Histogram | P99 > 500ms |
| `messages.parse_errors_total` | Counter | > 10/min |
| `reconciliation.discrepancy_count` | Gauge | > 0 on closed flight |
| `aodb.poll_latency` | Histogram | P95 > 5s |
| `hht.offline_queue_depth` | Gauge | > 50 per device |
| `api.error_rate` | Rate | > 1% over 5 min |

### 10.3 Structured Logging Convention

```csharp
// Every log entry includes these properties via middleware:
// - CorrelationId (X-Correlation-ID header)
// - UserId (from JWT sub claim)
// - MachineName

// Domain-specific structured fields:
logger.LogInformation(
    "Bag {BagTagNumber} status changed from {OldStatus} to {NewStatus} " +
    "for flight {FlightKey} by operator {OperatorId}",
    bag.TagNumber, oldStatus, newStatus, bag.FlightKey, operatorId);
```

---

## 11. Deployment Architecture

### 11.1 Production Layout

```
┌─────────────────────────────────────────────────────────────────┐
│                        Production Server                        │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │               NGINX (Reverse Proxy / TLS)               │   │
│  └─────────────────────────────┬───────────────────────────┘   │
│                                │                               │
│  ┌─────────────────────────────▼───────────────────────────┐   │
│  │              BRS Monolith (ASP.NET Core 10)              │   │
│  │        API + Blazor + Background Workers                 │   │
│  └─────────────────────────────┬───────────────────────────┘   │
│                                │                               │
│  ┌─────────────────────────────▼───────────────────────────┐   │
│  │                    PostgreSQL 16                         │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

### 11.2 CI/CD Pipeline

```
GitHub Push
    │
    ├── PR → Feature Branch
    │        └── Build + Unit Tests + Lint
    │
    └── Merge to main
         ├── Build + Integration Tests (Testcontainers)
         ├── Security scan (Trivy)
         ├── Deploy to staging
         ├── E2E Tests (Playwright)
         └── Deploy to production
```

---

## 12. Failure Modes & Resilience

| Failure | Impact | Mitigation |
|---|---|---|
| AODB offline | No new flight data | In-memory cache with 4h TTL, graceful degradation |
| IATA TCP connection drops | No new messages | `IataListenerWorker` reconnects with exponential backoff; raw TCP buffer preserved |
| Postgres outage | Full write outage | Polly retry + circuit breaker; read-only mode for dashboards |
| HHT loses Wi-Fi | Scan data loss | Offline SQLite queue, auto-sync on reconnect |
| Blazor SignalR disconnect | Dashboard goes stale | Auto-reconnect with exponential backoff, polling fallback |

---

## 13. Performance Targets

| Operation | Target (P99) | Notes |
|---|---|---|
| Scan submission (REST) | < 200ms | Online HHT scan |
| Batch scan flush | < 2s for 100 scans | Offline sync |
| BSM parse + persist | < 500ms | Full pipeline |
| Bag lookup by tag | < 50ms | Indexed query |
| Reconciliation query | < 1s | Indexed, pre-aggregated |
| Blazor dashboard load | < 2s | Initial render |
| Max concurrent HHTs | 200 devices | Single instance |
| Scan ingestion rate | ≥ 500 scans/sec | At P99 latency target |
| Messages/hour | ≥ 50,000 BSM | IATA worker throughput |

---

## 14. ADR Index

| # | Title | Status | Date |
|---|---|---|---|
| 001 | Use PostgreSQL | Accepted | 2025-01-10 |
| 002 | Monolith architecture | Accepted | 2025-01-15 |
| 005 | Blazor Server for dashboards, WASM for tools | Accepted | 2025-02-01 |
| 006 | MAUI offline-first with SQLite queue | Accepted | 2025-02-05 |
| 007 | Single DB with logical schema separation | Accepted | 2025-02-10 |
| 008 | ASP.NET Core JWT custom auth | Accepted | 2025-02-10 |
| 010 | Polly v8 for all HTTP resilience | Accepted | 2025-02-15 |
| 013 | NBomber for load testing | Accepted | 2025-03-05 |
| 014 | OpenTelemetry → Grafana stack | Accepted | 2025-03-10 |
