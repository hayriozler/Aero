# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

> AI assistant context for the **AeroScan** monorepo. Read this file before making any changes. BRS is a **monolithic** ASP.NET Core 10 application — all modules, workers, and the Blazor frontend live in a single deployable project.

---

## Project Overview

The **Baggage Reconciliation System (BRS)** is a mission-critical airport operations platform that reconciles passenger check-in data against physical baggage at every stage of the journey — from check-in desk to aircraft hold. It integrates with the Airport Operational Database (AODB), processes IATA standard messages (BSM, BPM, BTM via EDIFACT/Type B), drives handheld scanner terminals on the ramp, and exposes a management UI for the baggage office.

**Tech Stack**

| Layer | Technology |
|---|---|
| Backend API | .NET 10 · Minimal API · REST |
| Background Workers | .NET 10 `BackgroundService` (hosted within the monolith) |
| Frontend (Web) | Blazor 10 (Server + WASM hybrid) · MudBlazor |
| Mobile / HHT | .NET MAUI 10 (Android-first) |
| Database | PostgreSQL 16 · EF Core (latest) |
| Validation | FluentValidation |
| API Docs | Scalar |
| Reverse Proxy | NGINX |
| Observability | OpenTelemetry → Grafana / Loki / Tempo |
| Auth | ASP.NET Core JWT (built-in) — custom table names/schema required |

---

## Repository Layout

```
AeroScan/
├── CLAUDE.md                        ← YOU ARE HERE
├── ARCHITECTURE.md                  ← Full system design
├── AeroScan.slnx                    ← Solution file
│
├── src/
│   ├── AeroScan/                    ← Main monolith (API + Blazor + Workers)
│   │   ├── Program.cs
│   │   ├── Modules/
│   │   │   ├── Baggage/             ← Bag lifecycle, reconciliation
│   │   │   ├── Flights/             ← AODB feed, flight/passenger sync
│   │   │   ├── Scanning/            ← HHT scan ingestion & command dispatch
│   │   │   ├── Reporting/           ← Aggregations, CUSRES, audit trails
│   │   │   └── Messaging/           ← BSM/BPM/BTM EDIFACT parser
│   │   ├── Workers/
│   │   │   ├── AodbPollerWorker.cs
│   │   │   ├── IataListenerWorker.cs
│   │   │   └── ReconciliationWorker.cs
│   │   └── Infrastructure/          ← EF Core DbContexts, middleware, auth
│   │
│   ├── AeroScan.Domain/             ← Core domain entities & value objects
│   ├── AeroScan.Contracts/          ← Shared DTOs, request/response records, enums
│   │
│   ├── AeroScan.Web/                ← Blazor 10 hybrid app (Server + WASM)
│   │
│   └── AeroScan.Mobile/             ← .NET MAUI Android HHT app
│
└── scripts/
    ├── seed-db.sh
    ├── generate-edifact-samples.sh
    └── dev-setup.sh
```

---

## Development Commands

```bash
# Run the monolith
dotnet run --project src/AeroScan

# Build entire solution
dotnet build AeroScan.slnx

# Run all tests
dotnet test AeroScan.slnx

# Run a single test project
dotnet test src/AeroScan.Tests/AeroScan.Tests.csproj

# Run a single test by name filter
dotnet test --filter "FullyQualifiedName~BagCheckinShould"

# Add a new project to the solution
dotnet sln AeroScan.slnx add <path/to/Project.csproj>

# Add EF Core migration
dotnet ef migrations add 20260101_Description --project src/AeroScan --context BaggageDbContext

# First-time setup
./scripts/dev-setup.sh
```

---

## Architecture Principles

- Domain layer has **zero external dependencies**
- Application layer defines interfaces; Infrastructure layer implements them
- Database access through EF Core `DbContext` (Repository pattern optional)
- Modules communicate via direct method calls or in-process domain events — never via HTTP
- Use primary constructors everywhere possible
- Prefer `int`/`long` PKs over GUIDs

### 1. Domain-Driven Design (DDD)

- Modules own their bounded context but share the same database (logical schema separation via PostgreSQL schemas).
- In-process domain events are the integration contract between modules — never direct cross-module DbContext access.
- Aggregates: `Bag`, `Flight`, `Passenger`, `ScanEvent`, `ReconciliationRecord`.

### 2. Minimal API Pattern (.NET 10)

```csharp
// ✅ Correct — group endpoints with TypedResults
var bags = app.MapGroup("/api/bags").RequireAuthorization();

bags.MapGet("/{tagNumber}", async (
    string tagNumber,
    IBaggageRepository repo,
    CancellationToken ct) =>
{
    var bag = await repo.FindByTagAsync(tagNumber, ct);
    return bag is null ? TypedResults.NotFound() : TypedResults.Ok(bag);
})
.WithName("GetBagByTag")
.WithOpenApi()
.Produces<BagDto>()
.ProducesProblem(404);
```

- Use `TypedResults` not `Results` (compile-time safety).
- All endpoints must have `.WithName()` and `.WithOpenApi()`.
- Group related endpoints under `MapGroup` with shared auth/prefix.
- Use endpoint filters for validation (`IEndpointFilter`).

### 3. Layered Architecture

```
Domain         → entities, value objects, domain events, Result<T>
Contracts      → DTOs, request/response records, shared enums
Infrastructure → EF Core DbContexts, repositories, middleware
Modules        → application logic, minimal API endpoints
Workers        → BackgroundService implementations
```

### 4. IATA Message Processing

- IATA Type B / EDIFACT messages are received by `IataListenerWorker` over TCP.
- Each message is parsed synchronously and immediately handed to the relevant module.
- Failed parses are written to `brs_messaging.raw_messages` with `parse_status = FAILED` for manual review.
- **BSM** (Baggage Source Message) → creates/updates `Bag` in the Baggage module.
- **BPM** (Baggage Production Message) → updates sortation/carousel data.
- **BTM** (Baggage Transfer Message) → triggers inter-flight bag transfer workflow.

### 5. HHT Offline-First

- MAUI app uses SQLite for local state; syncs with the BRS API when connectivity is restored.
- Scan events are buffered locally with timestamps; server reconciles ordering on receipt.
- All scan operations must work completely offline; reconciliation is eventual.

### 6. Blazor Architecture

- Use **Server** mode for dashboard pages (real-time via SignalR).
- Use **WASM** mode for self-contained tools (report viewer, config pages).
- Component hierarchy: `Page → Layout → Feature → UI`.
- State: Fluxor or custom `CascadingValue`/`StateContainer` — no ad-hoc static fields.
- Keep WASM bundles < 8 MB; use lazy loading for report modules.

---

## Coding Standards

### General

- **C# 13** language features allowed everywhere.
- Nullable reference types **enabled** across all projects.
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in all `.csproj` files.
- Use `record` types for DTOs, value objects, and events.
- Use `readonly struct` for hot-path value objects (e.g., `BagTagNumber`, `FlightKey`).

### Naming

| Concept | Convention | Example |
|---|---|---|
| Domain Entity | PascalCase noun | `Bag`, `ScanEvent` |
| Domain Event | Past-tense + "Event" | `BagCheckedInEvent` |
| DTO | `Dto` suffix | `BagDto`, `FlightDto` |
| Request/Response | `Request` / `Response` suffix | `CheckInBagRequest` |
| Repository interface | `I` + Entity + `Repository` | `IBaggageRepository` |

### Error Handling

- Use `Result<T>` (from `Domain.Common`) — never throw for business rule failures.
- Use Problem Details (RFC 9457) for all API error responses.
- Global exception handler: `Infrastructure.Middleware.ExceptionHandlingMiddleware`.
- Never swallow exceptions silently.

### Logging

```csharp
// ✅ Correct — structured logging with semantic fields
logger.LogInformation("Bag {BagTagNumber} scanned at {Location} for flight {FlightKey}",
    scan.TagNumber, scan.Location, scan.FlightKey);

// ❌ Wrong — string interpolation loses structure
logger.LogInformation($"Bag {scan.TagNumber} scanned");
```

Bag tag numbers are PII-adjacent — redact in logs above DEBUG level.

### Database / EF Core

- Each module has its own `DbContext` scoped to its schema.
- Audit logging via EF Core interceptors; use a dedicated `brs_audit` schema.
- Always use **explicit transactions** for multi-table operations.
- Never use `Include()` chains deeper than 2 levels — use projections/views instead.
- All queries must be cancellation-token aware.
- Migration naming: `YYYYMMDD_ShortDescription` (e.g., `20250401_AddBagStatusIndex`).
- All migrations must have a `Down()` method.
- Use `HasIndex` with `IsUnique` for natural keys (bag tag number + flight key).
- Prefer `int`/`long` PKs; avoid GUIDs.
- **Never** run `dotnet ef database update` in production — use `MigrationRunner` at startup.

### Security

- All endpoints require JWT (`RequireAuthorization()`).
- Fine-grained policies defined in `Infrastructure.Auth.Policies`.
- JWT identity tables use custom names/schema (do not use ASP.NET Identity defaults).
- Never log bearer tokens, passwords, or PII.
- Input sanitization via FluentValidation on all Commands/Requests.

---

## Domain Model Quick Reference

```
Passenger ──< PassengerBag >── Bag
                                │
                          BagJourney (1..*)
                                │
                          ScanEvent (1..*)
                           │       │
                     Location    ScanType
                                (Check-in / Loading /
                                 Transfer / Arrival)

Flight ──< FlightBag >── Bag
  │
  ├── FlightLeg (1..*)
  └── Carousel

ReconciliationRecord
  ├── FlightKey
  ├── TotalBagsChecked
  ├── TotalBagsLoaded
  ├── UnmatchedBags [BagTagNumber]
  └── OffloadedBags [BagTagNumber]
```

---

## Key Environment Variables

```env
# Database
BRS__DB__CONNECTION=Host=localhost;Database=brs;Username=brs;Password=...

# AODB Integration
AODB__ENDPOINT=http://aodb-adapter:9090
AODB__POLL_INTERVAL_SECONDS=30

# IATA Type B / EDIFACT gateway
TYPEB__HOST=typeb-gateway.airport.local
TYPEB__PORT=5555
TYPEB__AIRLINE_CODE=XX

# JWT
JWT__SECRET=...
JWT__ISSUER=brs
JWT__AUDIENCE=brs-clients
JWT__EXPIRY_MINUTES=60
```

---

## Testing Strategy

| Layer | Tool | Coverage Target |
|---|---|---|
| Unit (domain, parsers) | xUnit + FluentAssertions | ≥ 90% |
| Integration (API + DB) | xUnit + Testcontainers | ≥ 75% |
| E2E (full flow) | Playwright + Blazor | Critical paths |
| Load | NBomber | Scan ingestion ≥ 500 rps |
| EDIFACT parser | Custom corpus of real samples | 100% message types |

- Integration tests spin up Postgres via **Testcontainers**.
- E2E tests run against a Docker Compose stack in CI.
- Load tests run nightly against a staging cluster.

---

## Common Gotchas

1. **EDIFACT line endings** — Type B messages use CRLF; some airline feeds send LF only. The parser normalises before tokenising.
2. **Bag tag number format** — IATA 10-digit (airline code 3 + serial 6 + check digit 1). Always validate with `BagTagNumber.TryParse()`, never raw string comparison.
3. **Flight key uniqueness** — A flight can appear twice (departure + arrival context). Always key by `FlightNumber + OriginDate + DepartureStation`.
4. **HHT clock drift** — Android devices on the ramp can drift ±30s. Scan events carry both device timestamp and server-receipt timestamp; reconciliation uses server time for ordering.
5. **AODB feed gaps** — AODB can go offline. The Flight module uses an in-memory cache with a TTL of 4 hours and degrades gracefully.
6. **BSM vs BPM timing** — BPM can arrive before the corresponding BSM. Always upsert, never assume arrival order.

---

## PR / Commit Guidelines

- **Branch**: `feature/<ticket>-short-description`, `fix/<ticket>-short-description`
- **Commit format**: Conventional Commits — `feat(scanning): add offline queue flush on reconnect`
- **PR size**: < 400 lines changed (excluding generated code and migrations)
- Every PR requires: passing CI, ≥ 1 review, no TODO/FIXME left in diff

---

## Contacts / Ownership

| Area | Owner Team |
|---|---|
| IATA parsing, AODB | Integration Team |
| Baggage Module, Reconciliation | Core Domain Team |
| Blazor Web App | Frontend Team |
| MAUI HHT | Mobile Team |
| Infrastructure, CI/CD | Platform Team |
