# AeroFlight — Airport Operational Database & Flight Management System

## Project Overview

Two deployable applications in a single solution, communicating via RabbitMQ:

- **AeroFlight** — monolithic AODB/Flight Management System for airport operators. Manages the full lifecycle of flight operations: scheduling, gate/belt/counter assignments, resource allocation, real-time status updates, and operational reporting. Also integrates with external systems (airlines, handling agents, upstream AODBs) via RabbitMQ.
- **AeroFids** — Flight Information Display System for passengers. Subscribes to flight events from AeroFlight via RabbitMQ and displays live departure/arrival boards.

---

## Architecture

```
AeroFlight/
├── src/
│   ├── AeroFlight.AppHost/          # .NET Aspire orchestration (starts all services, PostgreSQL, RabbitMQ)
│   ├── AeroFlight.ServiceDefaults/  # Shared telemetry, health checks, resiliency (used by all services)
│   │
│   │   ── AeroFlight (monolith) ──
│   ├── AeroFlight.Api/              # ASP.NET Core Minimal API — operator-facing HTTP endpoints
│   ├── AeroFlight.Web/              # Blazor Server — operator UI (MudBlazor)
│   ├── AeroFlight.Domain/           # Entities, value objects, domain interfaces
│   ├── AeroFlight.Application/      # ICommandHandler / IQueryHandler implementations, FluentValidation
│   ├── AeroFlight.Infrastructure/   # EF Core, RabbitMQ.Client publisher/consumer, PostgreSQL
│   │
│   │   ── AeroFids (passenger display) ──
│   ├── AeroFids.Service/             # Blazor Server admin + RabbitMQ consumer + REST/SignalR for display clients
│   │                                  Subscribes to AeroFlight events via RabbitMQ
│   │
│   └── AeroFlight.Contracts/        # Shared RabbitMQ message contracts (records only)
│       ├── Outbound/                # Events published by AeroFlight
│       ├── Inbound/                 # Messages consumed by AeroFlight from external systems
│       └── Topology/                # Exchange names, routing keys, queue names as constants
│
└── tests/
    ├── AeroFlight.UnitTests/
    ├── AeroFlight.IntegrationTests/
    └── AeroFlight.FunctionalTests/
```

---

## RabbitMQ Topology

All exchange/queue/routing-key names are defined as constants in `AeroFlight.Contracts/Topology/`.

### Outbound — AeroFlight publishes

Exchange: `aeroflight.flights` (topic)

| Routing Key | Event | Consumers |
|---|---|---|
| `flight.status.changed` | Flight status transition | AeroFids, external |
| `flight.arrival.updated` | Arrival flight data changed | AeroFids, external |
| `flight.departure.updated` | Departure flight data changed | AeroFids, external |
| `flight.gate.assigned` | Gate allocation changed | AeroFids, external |
| `flight.belt.assigned` | Baggage belt changed | AeroFids, external |
| `flight.counter.assigned` | Check-in counter changed | AeroFids, external |
| `flight.delay.added` | Delay code recorded | AeroFids, external |
| `flight.cancelled` | Flight cancelled | AeroFids, external |
| `flight.diverted` | Flight diverted | AeroFids, external |

### Inbound — AeroFlight consumes from external systems

Exchange: `aeroflight.inbound` (topic)

| Routing Key | Message | Action |
|---|---|---|
| `schedule.arrival.received` | Seasonal arrival schedule | Import/update ArrivalFlight records |
| `schedule.departure.received` | Seasonal departure schedule | Import/update DepartureFlight records |
| `mvt.received` | IATA MVT message | Update actual times, status |
| `ldm.received` | IATA LDM message | Update baggage/load data |
| `metadata.flight.updated` | External metadata update | Update flight supplementary fields |
| `movement.message.received` | Raw movement message for a flight | Persist as read-only MovementMessage record (no field parsing) |

### AeroFids queues

| Queue | Bound to | Purpose |
|---|---|---|
| `aerofids.flight-updates` | `aeroflight.flights.*` | All flight events for display boards |

---

## Technology Stack

### AeroFlight (monolith)
- **Runtime:** .NET 10 (ASP.NET Core)
- **API:** ASP.NET Core Minimal APIs (operator-facing)
- **UI:** Blazor Server + MudBlazor (operator UI)
- **Architecture:** Clean Architecture — Domain → Application → Infrastructure → Api/Web
- **CQRS:** Custom `ICommandHandler<TCommand>` / `IQueryHandler<TQuery, TResult>` — no MediatR
- **Validation:** FluentValidation
- **Mapping:** none — manual projection inside handlers
- **ORM:** Entity Framework Core + Npgsql (PostgreSQL)
- **Messaging:** RabbitMQ.Client behind `IMessageBus` — publishes outbound events, consumes inbound messages

### AeroFids
- **Runtime:** .NET 10 (ASP.NET Core)
- **UI:** Blazor Server + MudBlazor (passenger-facing display boards)
- **Messaging:** RabbitMQ.Client — subscribes to `aeroflight.flights.*`
- **State:** In-memory flight state rebuilt from events; no own database

### Shared
- **.NET Aspire:** Single AppHost orchestrates both apps, PostgreSQL, and RabbitMQ
- **ServiceDefaults:** Shared by all services for OpenTelemetry, health checks, resiliency
- **Contracts:** `AeroFlight.Contracts` project referenced by both AeroFlight and AeroFids

---

## Package Policy

**All NuGet packages must be free and open-source (MIT, Apache 2.0, or equivalent). No commercial or source-available licenses.**

Approved packages (non-exhaustive):
| Package | License | Purpose |
|---|---|---|
| RabbitMQ.Client | Apache 2.0 | RabbitMQ official .NET client |
| FluentValidation | Apache 2.0 | Input validation |
| Npgsql.EntityFrameworkCore.PostgreSQL | MIT | EF Core PostgreSQL provider |
| MudBlazor | MIT | Blazor UI components |
| Serilog | Apache 2.0 | Structured logging |
| OpenTelemetry | Apache 2.0 | Distributed tracing/metrics |
| Testcontainers | MIT | Containers for integration tests |
| xunit | Apache 2.0 | Test framework |
| FluentAssertions | Apache 2.0 | Test assertions |
| NSubstitute | BSD 3-Clause | Mocking |
| Bogus | MIT | Fake test data |

**Central Package Management:** All package versions are defined in `Directory.Packages.props` at the solution root. Project files use `<PackageReference>` without a `Version` attribute. Never set a version in a `.csproj` file.

**Before adding any new package:** verify its license on NuGet.org or the project's GitHub repository.

---

## Domain Concepts

- **Flight:** A scheduled aircraft movement (arrival or departure) with an IATA flight number, airline, origin/destination, scheduled/estimated/actual times.
- **Stand / Gate:** Airport resource assigned to a flight for a time window.
- **Resource Allocation:** The act of assigning a stand, gate, check-in desk, baggage belt, etc. to a flight.
- **Flight Status:** One of SCHEDULED → BOARDING → DEPARTED / LANDED → CANCELLED / DIVERTED.
- **AODB Message:** An operational message (e.g., MVT, LDM) exchanged with airlines or handling agents.
- **Slot:** IATA-assigned time window for take-off or landing.
- **FIDS:** Flight Information Display System — passenger-facing boards showing live flight status.

---

## Coding Conventions

- Follow Clean Architecture boundaries: Domain has no dependencies on Infrastructure or Application.
- Use records for DTOs, message contracts, and value objects; use classes for entities and aggregates.
- Domain events are dispatched by the application layer via `IEventDispatcher` implemented in Infrastructure; cross-system events go through RabbitMQ via `IMessageBus`.
- Endpoints must not contain business logic — delegate to `ICommandHandler` / `IQueryHandler` via DI.
- Never expose EF Core entities directly from API responses; project to response records inside the handler.
- Use `CancellationToken` on every async method signature.
- Prefer `IResult` returns in Minimal API endpoints.
- No `async void` — use `async Task` everywhere.
- No object mapping libraries — construct response/DTO records manually inside handlers.
- No comments unless the WHY is non-obvious and cannot be expressed through naming. One short line maximum — no comment blocks, no summaries, no XML docs on internal types.

---

## Testing Requirements

Integration and testing are **mandatory**, not optional.

### Unit Tests
- Cover all domain logic, application handlers, and validators.
- No infrastructure dependencies — mock everything with NSubstitute.
- Run fast: no I/O, no network, no database.

### Integration Tests
- Use `WebApplicationFactory<Program>` for API tests.
- Use Testcontainers to spin up real PostgreSQL and RabbitMQ instances per test collection.
- Test all critical API endpoints, EF Core repository implementations, and RabbitMQ consumers.
- Assert against a real database state, not mocks.

### Functional / E2E Tests
- Use Playwright for browser-level tests on Blazor Server flows.
- Scope to critical user journeys: flight search, status update, resource assignment, FIDS display refresh.

### Coverage Target
- Domain + Application layers: ≥ 90% line coverage.
- Infrastructure adapters: covered by integration tests, not unit tests.

---

## .NET Aspire Setup Notes

- `AeroFlight.AppHost` is the single entry point for local development — starts AeroFlight.Api, AeroFlight.Web, AeroFids.Service, PostgreSQL, and RabbitMQ.
- All services reference `AeroFlight.ServiceDefaults` for consistent telemetry, health, and resiliency.
- Do not hard-code connection strings or RabbitMQ URIs — use Aspire service discovery and configuration injection.

---

## Environment & Configuration

- Configuration via `appsettings.json` + environment variable overrides (ASP.NET Core standard).
- Secrets must never be committed — use `dotnet user-secrets` locally, environment variables in production.
- No `.env` files in the repository.

---

## Git Commit Conventions

- Subject line: ≤ 50 characters, imperative mood, no period. Example: `add flight status endpoint`
- No body unless the change is non-obvious — keep it to one line when possible.
- Prefix with type only when it adds clarity: `fix`, `add`, `remove`, `update`, `refactor`, `test`, `chore`.
- Never reference ticket numbers or issue IDs in the subject line.

---

## What to Avoid

- No commercial NuGet packages (e.g., Telerik, Syncfusion, DevExpress, AutoMapper 13+).
- No MediatR — adds indirection; newer versions require a commercial license.
- No MassTransit or EasyNetQ — use RabbitMQ.Client directly behind `IMessageBus`.
- No object mapping libraries (AutoMapper, Mapster, etc.) — project manually inside handlers.
- No `Thread.Sleep` or blocking calls in async code.
- No static state or singleton services that hold mutable shared state.
- Do not bypass `IMessageBus` and call RabbitMQ.Client channel APIs directly from application code.
- No God classes or God services — keep handlers and services focused.
- No `dynamic` types.
- Do not suppress nullable warnings with `!` unless the null impossibility is proven by invariant.
- No comment blocks, multi-line comments, or XML doc comments on internal code. One short inline comment at most.
