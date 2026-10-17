# AeroFlight Backend Architecture — Reference Data & Ops

This document explains, in detail, how the two main endpoint families in `AeroFlight.Api` are built:

- **Reference Data** — the generic CRUD/lookup engine behind every dropdown-backing table (Countries, Airports, Airlines, Gate Types, Time Zones, ...).
- **Ops** — the flight-operations domain (Flights, Gates/Belts/Counters, Routing, Delays, Park Positions), which follows the same CQRS foundation but is largely hand-written per feature rather than generic.

Both sit on the same Clean Architecture layering: `Domain` → `Application` → `Infrastructure` → `Api`/`Web`.

---

## 1. Shared foundation

### 1.1 CQRS interfaces

Every operation in the system — reference data or ops — is a command or a query handled by one of two interfaces (`AeroFlight.Application/Common`):

```csharp
public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct);
}

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct);
}
```

There is no MediatR. Handlers are resolved directly from DI as the closed generic interface, e.g. `IQueryHandler<GetFlightsQuery, PagedResult<MergedFlightRow>>`.

### 1.2 Response envelope — `ApiResult`

Every Minimal API endpoint returns one of the static methods on `ApiResult` (`AeroFlight.Api/Infrastructure/ApiResult.cs`). Endpoints never return `Results.Ok`/`Results.NotFound` directly.

| Method | HTTP | Notes |
|---|---|---|
| `ApiResult.Ok(ctx, data)` | 200 | Successful GET/PUT |
| `ApiResult.Created(ctx, data, location)` | 201 | Sets the `Location` header |
| `ApiResult.Paged(ctx, data, pagination)` | 200 | Wraps a page of results + `ApiPagination` |
| `ApiResult.NoContent(ctx)` | 204 | DELETE |
| `ApiResult.NotFound(ctx, code, message)` | 404 | |
| `ApiResult.Conflict(ctx, code, message)` | 409 | Duplicate / business-rule violation |
| `ApiResult.ValidationFailed(ctx, fluentValidationResult)` | 422 | Every error gets code `VALIDATION_ERROR` |
| `ApiResult.Unauthorized(ctx)` | 401 | |

Every response is JSON-shaped as:

```json
{
  "requestStartTime": "...",
  "requestFinishTime": "...",
  "httpStatus": 200,
  "traceId": "...",
  "data": { },
  "pagination": null,
  "errors": null
}
```

### 1.3 Where endpoints are wired up

`AeroFlight.Api/Program.cs` calls one `Map*Endpoints()` extension method per feature, all inside a tenant-scoped route group:

```csharp
tenant.MapAuthEndpoints();
tenant.MapUserEndpoints();
tenant.MapRoleEndpoints();
tenant.MapReferenceDataEndpoints();   // ← Reference Data (§2)
tenant.MapBusinessRuleEndpoints();
tenant.MapFlightEndpoints();          // ← Ops (§3)
tenant.MapFlightResourceEndpoints();
tenant.MapFlightRoutingEndpoints();
tenant.MapFlightParkPositionEndpoints();
tenant.MapFlightCodeShareEndpoints();
tenant.MapFlightDelayEndpoints();
tenant.MapFlightFieldEndpoints();
tenant.MapResourceEndpoints();
```

Each `Map*Endpoints()` extension method lives in `AeroFlight.Api/Endpoints/<Feature>/` and is a thin `IEndpointRouteBuilder` extension — no logic, just route → handler wiring.

---

## 2. Reference Data subsystem

Reference Data is a **generic engine**, not per-entity hand-written code. Adding a new lookup table (e.g. `ServiceType`) touches ~8 small files, all following an identical shape, and none of them contain endpoint or caching logic — that's all generic and shared.

### 2.1 Two flavors

| Flavor | Registered via | Endpoints | Example entities |
|---|---|---|---|
| **CRUD** | `AddReferenceCrud<TEntity, TRequest, TResponse, TMap, TLog>()` | GET list, GET one, GET audit, POST, PUT, DELETE | `Country`, `Airport`, `Airline`, `Gate`, `Aircraft` |
| **Lookup (read-only)** | `AddCachedReferenceLookup<TEntity, TResponse, TReader>()` | GET list, GET one only | `CountryType`, `GateType`, `Region`, `ServiceType`, `Timezone`, `FlightStatus` |

A Lookup entity has no `Create`/`Update` domain methods and no audit log type — it's pure seed data (`AeroFlight.Migrations/Seeds/*.csv`), viewed but never edited through the API (some are still editable via the generic dialog if the Web catalog marks them so — see §2.6 — but most are seed-only).

### 2.2 The per-entity contract: `IReferenceDataMap` / `IReferenceDataReader`

```csharp
// AeroFlight.Application/ReferenceData/IReferenceDataMap.cs
public interface IReferenceDataReader<TEntity, TResponse> where TEntity : class
{
    string GetCode(TEntity entity);
    TResponse ToResponse(TEntity entity);

    // Translatable counterparts — let EF project straight to TResponse in SQL
    // instead of loading full entities and mapping in memory.
    Expression<Func<TEntity, string>> CodeSelector { get; }
    Expression<Func<TEntity, TResponse>> Projection { get; }
}

public interface IReferenceDataMap<TEntity, TRequest, TResponse>
    : IReferenceDataReader<TEntity, TResponse> where TEntity : class
{
    TEntity Create(TRequest request);
    void Apply(TEntity entity, TRequest request);
}
```

- **Lookup** entities implement only `IReferenceDataReader` (a `*Reader` class, e.g. `ServiceTypeReader`, `RegionReader`).
- **CRUD** entities implement the full `IReferenceDataMap` (a `*ReferenceMap` class, e.g. `CountryReferenceMap`, `AirportReferenceMap`), which also carries the `AbstractValidator<TRequest>` for that entity in the same file.

Example (`AeroFlight.Application/ReferenceData/AirportReferenceData.cs`):

```csharp
public sealed class AirportRequestValidator : AbstractValidator<AirportRequest>
{
    public AirportRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.TimezoneCode).MaximumLength(50);
        // ...
    }
}

public sealed class AirportReferenceMap : IReferenceDataMap<Airport, AirportRequest, AirportResponse>
{
    public string GetCode(Airport entity) => entity.Id;
    public Airport Create(AirportRequest r) => Airport.Create(r.Code, r.IataCode, r.Name, r.DisplayName);
    public void Apply(Airport e, AirportRequest r) =>
        e.Update(r.IataCode, r.IcaoCode, r.Name, r.DisplayName, r.CountryCode, r.TimezoneCode);
    public AirportResponse ToResponse(Airport e) =>
        new(e.Id, e.IataCode, e.IcaoCode, e.Name, e.DisplayName, e.CountryCode, e.TimezoneCode);

    public Expression<Func<Airport, string>> CodeSelector { get; } = e => e.Id;
    public Expression<Func<Airport, AirportResponse>> Projection { get; } = e => new AirportResponse(
        e.Id, e.IataCode, e.IcaoCode, e.Name, e.DisplayName, e.CountryCode, e.TimezoneCode);
}
```

### 2.3 Generic command/query handlers

These operate on `TEntity`/`TRequest`/`TResponse` and never change per entity:

| Class | File | Role |
|---|---|---|
| `ReferenceListQueryHandler<TEntity, TResponse>` | `ReferenceQueryHandlers.cs` | `SELECT ... ORDER BY CodeSelector` projected via `Projection` |
| `ReferenceGetQueryHandler<TEntity, TResponse>` | `ReferenceQueryHandlers.cs` | `WHERE CodeSelector == id` projected via `Projection` |
| `ReferenceCreateCommandHandler<TEntity, TRequest, TResponse>` | `ReferenceCommandHandlers.cs` | `map.Create` → `map.Apply` → `SaveChangesAsync` |
| `ReferenceUpdateCommandHandler<TEntity, TRequest, TResponse>` | `ReferenceCommandHandlers.cs` | `db.FindAsync` → `map.Apply` → `SaveChangesAsync` |
| `ReferenceDeleteCommandHandler<TEntity>` | `ReferenceCommandHandlers.cs` | `db.FindAsync` → `db.Remove` → `SaveChangesAsync` |
| `ReferenceAuditQueryHandler<TEntity, TLog>` | (Infrastructure) | Reads the entity's audit log table (see §2.7) |

The commands/queries themselves are equally generic (`ReferenceMessages.cs`):

```csharp
public sealed record ReferenceListQuery<TEntity>;
public sealed record ReferenceGetQuery<TEntity>(string Id);
public sealed record ReferenceCreateCommand<TEntity, TRequest>(TRequest Request);
public sealed record ReferenceUpdateCommand<TEntity, TRequest>(string Id, TRequest Request);
public sealed record ReferenceDeleteCommand<TEntity>(string Id);
```

### 2.4 Caching layer — read side

Every reference-data read goes through `IMemoryCache`, wrapped one layer above the tracked handlers above (`AeroFlight.Infrastructure/ReferenceData/CachedReferenceQueryHandlers.cs`):

```csharp
internal static class ReferenceCacheDuration
{
    public static readonly TimeSpan Lookup = TimeSpan.FromHours(12); // pure lookups never change
    public static readonly TimeSpan Crud = TimeSpan.FromSeconds(30); // CRUD entities — short window
}

internal static class ReferenceCacheKeys
{
    public static string List<TEntity>(string tenantId) => $"refdata:{tenantId}:{typeof(TEntity).Name}:list";
    public static string Get<TEntity>(string tenantId, string id) => $"refdata:{tenantId}:{typeof(TEntity).Name}:get:{id}";
}
```

| Class | Wraps | Cache key | Duration |
|---|---|---|---|
| `CachedReferenceListQueryHandler<TEntity, TResponse>` | Reads DB directly (`AsNoTracking`) — for **Lookup** entities only | `List<TEntity>` | `Lookup` (12h) |
| `CachedCrudListQueryHandler<TEntity, TResponse>` | `ReferenceListQueryHandler<TEntity, TResponse>` — for **CRUD** entities | `List<TEntity>` | `Crud` (30s) |
| `CachedReferenceGetQueryHandler<TEntity, TResponse>` | `ReferenceGetQueryHandler<TEntity, TResponse>` — shared by both flavors, only the `duration` ctor param differs | `Get<TEntity>(id)` | `Lookup` or `Crud` |

Cache keys are **tenant-scoped** (`ITenantContext.TenantId` is part of the key) — each tenant's Postgres database has its own cache entries even though they share the in-process `IMemoryCache`.

### 2.5 Caching layer — write side (invalidate-on-write)

`AeroFlight.Infrastructure/ReferenceData/CachedReferenceCommandHandlers.cs` wraps the three write handlers the same way — run the inner handler, then evict exactly the keys `ReferenceCacheKeys` would have used for that entity:

```csharp
internal sealed class CachedReferenceUpdateCommandHandler<TEntity, TRequest, TResponse>(
    ReferenceUpdateCommandHandler<TEntity, TRequest, TResponse> inner,
    IMemoryCache cache, ITenantContext tenant)
    : ICommandHandler<ReferenceUpdateCommand<TEntity, TRequest>, TResponse?>
    where TEntity : class
{
    public async Task<TResponse?> HandleAsync(ReferenceUpdateCommand<TEntity, TRequest> command, CancellationToken ct)
    {
        var result = await inner.HandleAsync(command, ct);
        cache.Remove(ReferenceCacheKeys.List<TEntity>(tenant.TenantId));
        cache.Remove(ReferenceCacheKeys.Get<TEntity>(tenant.TenantId, command.Id));
        return result;
    }
}
```

`Create` only evicts the list key (nothing was cached under the new entity's id yet); `Update`/`Delete` evict both. This means an edit is visible on the very next request — the 30s `Crud` duration in §2.4 now only bounds how long an entry that was *never* written to lingers in memory, not how stale a live edit can look.

### 2.6 DI registration — the two entry points

`AeroFlight.Infrastructure/ReferenceData/ReferenceDataServiceCollectionExtensions.cs`, called once from `AddReferenceDataEntities()`:

```csharp
// CRUD entity — full read/write + audit
services.AddReferenceCrud<Airport, AirportRequest, AirportResponse, AirportReferenceMap, AirportAudit>();

// Read-only lookup — no Create/Update/Delete, long cache
services.AddCachedReferenceLookup<Region, RegionResponse, RegionReader>();
```

`AddReferenceCrud<TEntity, TRequest, TResponse, TMap, TLog>()`:

1. Calls `AddReferenceLookup<TEntity, TResponse, TMap>()` (registers the cached list/get query handlers — `TMap` doubles as the reader since `IReferenceDataMap` extends `IReferenceDataReader`).
2. Registers `TMap` as `IReferenceDataMap<...>` (singleton — it's stateless).
3. Registers the three tracked command handlers as concrete types, then registers their `Cached*CommandHandler` wrapper against the `ICommandHandler<...>` interface (so callers always resolve the caching wrapper, never the raw handler directly).
4. Registers `IReferenceInboundApplier`/`IReferenceOutboxMapper` (RabbitMQ inbound/outbound sync — out of scope for this document) and the audit query handler.

`AddCachedReferenceLookup<TEntity, TResponse, TReader>()` registers only the reader and the long-duration cached list/get query handlers — no command handlers exist for these entities at all.

### 2.7 Endpoint mapping

`AeroFlight.Api/Endpoints/ReferenceData/ReferenceEndpoints.cs` — the generic endpoint factory:

```csharp
app.MapReferenceCrudEndpoints<Country, CountryRequest, CountryResponse>(
    "/api/reference/countries", "Reference: Countries", r => r.Code);

app.MapReferenceLookupEndpoints<Region, RegionResponse>(
    "/api/reference/regions", "Reference: Regions");
```

`MapReferenceCrudEndpoints<TEntity, TRequest, TResponse>(route, tag, getRequestCode)`:

| Route | Handler resolved |
|---|---|
| `GET  {route}/` | `IQueryHandler<ReferenceListQuery<TEntity>, IReadOnlyList<TResponse>>` |
| `GET  {route}/{id}` | `IQueryHandler<ReferenceGetQuery<TEntity>, TResponse?>` |
| `GET  {route}/{id}/audit` | `IQueryHandler<ReferenceAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>` |
| `POST {route}/` | `ICommandHandler<ReferenceCreateCommand<TEntity, TRequest>, TResponse>` + `IValidator<TRequest>` |
| `PUT  {route}/{id}` | `ICommandHandler<ReferenceUpdateCommand<TEntity, TRequest>, TResponse?>` + `IValidator<TRequest>` |
| `DELETE {route}/{id}` | `ICommandHandler<ReferenceDeleteCommand<TEntity>, bool>` |

`MapReferenceLookupEndpoints<TEntity, TResponse>(route, tag)` maps only the two `GET` routes above — no `IValidator`, no write routes exist for it to call even if someone tried.

Every route requires one of `Permissions.ReferenceView/Create/Update/Delete`. All 60+ reference routes are registered in one file, `AeroFlight.Api/Endpoints/ReferenceData/ReferenceDataEndpointRegistration.cs`, which is the single list of every `/api/reference/*` route that exists.

### 2.8 Web side — one generic page + dialog for every entity

`AeroFlight.Web/Services/ReferenceData/ReferenceCatalog.cs` is the **single source of truth** for every reference route and its editable fields — nothing else in the Web project should hardcode a `/api/reference/*` string.

```csharp
public enum RefFieldType { Text, Number, Decimal, Bool, Select }
public sealed record RefField(string Name, string Label, RefFieldType Type,
    bool Required = false, int? MaxLength = null, string? OptionsRoute = null);
public sealed record RefColumn(string Name, string Label);
public sealed record ReferenceEntity(string Key, string Title, string Group, string Icon,
    string Route, bool ReadOnly, IReadOnlyList<RefColumn> Columns, IReadOnlyList<RefField> Fields);
```

One catalog entry per entity, e.g.:

```csharp
new("countries", "Countries", _geoGroup, Icons.Material.Filled.Flag, "/api/reference/countries", false,
    [C("code", "Code"), C("name", "Name"), C("countryTypeCode", "Type")],
    [T("code", "Code", true, 10), T("name", "Name", true, 200),
     S("countryTypeCode", "Country Type Code", "/api/reference/countrytypes")]),
```

`RefField.Name` **must exactly match the JSON property name** the API expects (camelCase of the C# `Request` property, e.g. `TimezoneCode` → `"timezoneCode"`) — the generic edit dialog builds the request body as a `Dictionary<string, object?>` keyed by `field.Name`, so any mismatch here silently drops that field on save without an error.

`RefFieldType.Select` fields (`S(...)` helper) render as a `MudSelect` whose options are fetched from `OptionsRoute` at dialog-open time — this is how "Country Type", "Country" (on Airports/Airlines), "Parent Airline", "Service Type", "Timezone", "Size/Noise Category" etc. all became dropdowns instead of free text.

Two Blazor components drive every single reference-data screen — there is no per-entity Razor page:

- `Pages/Reference/ReferenceDataPage.razor` (`@page "/reference/{Key}"`) — generic `MudDataGrid` over `ReferenceCatalog.ByKey(Key)`, with Add/Edit/Delete/History buttons hidden when `Entity.ReadOnly` is true.
- `Pages/Reference/ReferenceEditDialog.razor` — generic create/edit form, switching on `field.Type` to render `MudTextField`/`MudNumericField`/`MudSwitch`/`MudSelect`. Loads every `Select` field's options concurrently before first render (`_ready` flag) — rendering a `MudSelect` before its options exist leaves it permanently blank even once the options arrive on a later render, so the whole dialog body waits behind one `_ready` gate.

### 2.9 End-to-end: everything one new reference entity touches

Using `ServiceType` (added this session) as the concrete example — 3 rows, no UI screen requested:

| Layer | File | Content |
|---|---|---|
| Domain | `AeroFlight.Domain/Metadata/ServiceType.cs` | `{ Code, Description }`, no `Update` method (read-only) |
| Infrastructure (EF config) | `.../Configurations/ReferenceData/ServiceTypeConfiguration.cs` | `ToTable`, `HasKey`, column types |
| Contracts | `AeroFlight.Contracts/ReferenceData/ServiceTypeDtos.cs` | `ServiceTypeResponse(Code, Description)` |
| Application | `AeroFlight.Application/ReferenceData/ServiceTypeReferenceData.cs` | `ServiceTypeReader : IReferenceDataReader<...>` |
| Infrastructure (DI) | `ReferenceDataServiceCollectionExtensions.cs` | `AddCachedReferenceLookup<ServiceType, ServiceTypeResponse, ServiceTypeReader>()` |
| Api | `ReferenceDataEndpointRegistration.cs` | `MapReferenceLookupEndpoints<ServiceType, ServiceTypeResponse>("/api/reference/servicetypes", ...)` |
| Seed | `AeroFlight.Migrations/Seeds/service_types.csv` | `code,description` rows |
| Seeder | `ResourceSeeder.cs` | `SeedServiceTypesAsync` — insert-only-if-missing |
| Web catalog (only if it needs to be a combo source or browsable) | `ReferenceCatalog.cs` | `S("serviceTypeCode", "Service Type Code", "/api/reference/servicetypes")` on `Airline`'s fields, plus an optional `Lookup("servicetypes", ...)` entry to make it browsable |

A **CRUD** entity (like `Airport`) additionally needs: a domain `Update()` method, an `*Audit` domain class + its own `BaseAuditConfiguration<T>` config, `builder.HasAuditType<Airport, AirportAudit>()` in the EF config, a `Request` DTO + `*RequestValidator`, and `MapReferenceCrudEndpoints` instead of `MapReferenceLookupEndpoints`.

**Migration gotcha**: if a new lookup table is referenced by an FK from an **already-populated** column, `AddForeignKey` runs before the seeder ever gets a chance to populate the new lookup table — the constraint fails on the very rows that already exist. Fix: seed the lookup table's rows directly inside the migration (`migrationBuilder.Sql("INSERT INTO ... ON CONFLICT DO NOTHING")`) before the `AddForeignKey` call, rather than relying on the app-level seeder. Brand-new nullable columns added in the same migration don't have this problem — every existing row gets `NULL`, which always satisfies a FK.

---

## 3. Ops subsystem

Ops covers `ArrivalFlight`/`DepartureFlight` and everything hanging off them: Gates, Belts, Counters, Park Positions, Routing legs, Delays, Code Shares. Unlike Reference Data, most of this is **hand-written per feature** — there's no single generic engine — but two sub-patterns repeat throughout:

1. **Direction-generic resource CRUD** — one set of command/handler classes parameterized by `TResource : ResourceBase`, instantiated once per concrete resource type (`OpsArrivalGate`, `OpsDepartureGate`, `OpsArrivalBelt`, `OpsDepartureBelt`, `OpsCounter`).
2. **Flight-specific handlers** — one handler class per operation, not generic (`CreateArrivalFlightCommandHandler`, `UpdateDepartureFlightCommandHandler`, ...), because arrival/departure flights genuinely have different fields.

### 3.1 Flight CRUD (non-generic)

`AeroFlight.Api/Endpoints/Flights/FlightEndpoints.cs` maps `/api/flights/*`:

```csharp
group.MapGet("/", ...);                          // paged list — GetFlightsQuery → PagedResult<MergedFlightRow>
group.MapGet("/arrivals/{id:long}", ...);         // GetArrivalFlightDetailQuery
group.MapGet("/departures/{id:long}", ...);       // GetDepartureFlightDetailQuery
group.MapGet("/arrivals/{id:long}/audit", ...);   // GetFlightFullAuditQuery(id, IsArrival: true)
group.MapGet("/departures/{id:long}/audit", ...); // GetFlightFullAuditQuery(id, IsArrival: false)
group.MapPost("/arrivals", ...);                  // CreateArrivalFlightCommand
group.MapPost("/departures", ...);                // CreateDepartureFlightCommand
// + PUT/DELETE per direction
```

Each `Create*`/`Update*`/`Delete*FlightCommandHandler` lives in its own file under `AeroFlight.Application/Flights/` and, in addition to the EF `SaveChangesAsync`, typically also:

- Resolves `CountryType` via `IFlightCountryTypeResolver` (worst-severity-wins across the flight's routing legs — see §3.4 for the bug this had).
- Runs the Business Rules engine: `await ruleEngine.EvaluateAsync(RuleTrigger.ArrivalCreated /* or */ FlightUpdated, flight, ct)`.
- Attempts auto-linking the paired arrival/departure by aircraft registration: `await autoLinker.TryLinkArrivalAsync(flight, ct)`.

None of this is generic — each handler explicitly calls what it needs.

### 3.2 Direction-generic resource CRUD

`AeroFlight.Api/Endpoints/Flights/FlightResourceEndpoints.cs` registers the **same** endpoint set five times, once per concrete resource type, via a private generic helper:

```csharp
private static void MapResourceEndpoints<TResource>(IEndpointRouteBuilder app, string basePath)
    where TResource : ResourceBase
{
    app.MapGet(basePath, ...);                        // ListFlightResourcesQuery<TResource>
    app.MapPost(basePath, ...);                        // AddFlightResourceCommand<TResource>
    app.MapPut($"{basePath}/{{resourceId:long}}", ...); // UpdateFlightResourceCommand<TResource>
    app.MapDelete($"{basePath}/{{resourceId:long}}", ...); // DeleteFlightResourceCommand<TResource>
}

public static IEndpointRouteBuilder MapFlightResourceEndpoints(this IEndpointRouteBuilder app)
{
    MapResourceEndpoints<OpsArrivalGate>(app, "/api/flights/arrivals/{flightId:long}/gates");
    MapResourceEndpoints<OpsArrivalBelt>(app, "/api/flights/arrivals/{flightId:long}/arrivalbelts");
    MapResourceEndpoints<OpsDepartureGate>(app, "/api/flights/departures/{flightId:long}/gates");
    MapResourceEndpoints<OpsDepartureBelt>(app, "/api/flights/departures/{flightId:long}/departurebelts");
    MapResourceEndpoints<OpsCounter>(app, "/api/flights/departures/{flightId:long}/counters");
    return app;
}
```

The command/query side (`AeroFlight.Application/Flights/Resources/FlightResourceCommands.cs`) mirrors this — one generic set of records + handlers parameterized by `TResource : ResourceBase`:

```csharp
public sealed record AddFlightResourceCommand<TResource>(long FlightId, FlightResourceRequest Request) where TResource : ResourceBase;

public sealed class AddFlightResourceCommandHandler<TResource>(IAppDbContext db, IFlightResourceFactory<TResource> factory)
    : ICommandHandler<AddFlightResourceCommand<TResource>, FlightResourceResponse>
    where TResource : ResourceBase
{
    public async Task<FlightResourceResponse> HandleAsync(AddFlightResourceCommand<TResource> command, CancellationToken ct)
    {
        var resource = factory.Create(command.FlightId, command.Request.Code);
        Apply(resource, command.Request);
        db.Add(resource);
        await db.SaveChangesAsync(ct);
        return ListFlightResourcesQueryHandler<TResource>.ToResponse(resource);
    }
    // Apply() is also reused directly by UpdateFlightResourceCommandHandler<TResource>
}
```

`IFlightResourceFactory<TResource>` (`AeroFlight.Application/Flights/Resources/FlightResourceFactories.cs`) is the one piece that genuinely differs per concrete type — it knows how to call e.g. `OpsArrivalGate.CreateForArrival(flightId, code)` vs `OpsDepartureGate.CreateForDeparture(...)`. Five tiny factory classes, one per resource type, each registered in DI:

```csharp
// DependencyInjectionService.cs
services.AddFieldUpdateCrud<ArrivalFlight>(); // pattern repeats for each TResource too
```

Adding a **sixth** resource type (say, a new `OpsJetbridge`) means: one factory class, one `MapResourceEndpoints<OpsJetbridge>(...)` call, one DI registration line — the command/handler/endpoint logic itself needs no new code.

### 3.3 One-off sub-resources (not generic)

Routing (`RoutingCommands.cs`), Park Position (`ParkPositonCommands.cs`), Delay, and Code Share follow the same *shape* as §3.2 (List/Add/Update/Delete records + handlers, injected `IAppDbContext`) but are **not** type-parameterized — each is its own concrete set of classes, because their domain rules differ enough that genericizing them wasn't worth it (e.g. Park Position's `OpsFlightParkPosition` can belong to an arrival, a departure, or both — a shape none of the other resources have).

Example — adding a routing leg also triggers a side-effect the generic resource CRUD doesn't have (`AeroFlight.Application/Flights/Routings/RoutingCommands.cs`):

```csharp
public sealed class AddRoutingCommandHandler<TRouting>(
    IAppDbContext db, IRoutingFactory<TRouting> factory, IRoutingCountryTypeUpdater<TRouting> countryTypeUpdater)
    : ICommandHandler<AddRoutingCommand<TRouting>, AddRoutingResult>
    where TRouting : OpsRouting
{
    public async Task<AddRoutingResult> HandleAsync(AddRoutingCommand<TRouting> command, CancellationToken ct)
    {
        // ... create + save the leg ...
        await countryTypeUpdater.RefreshAsync(command.FlightId, ct); // recompute Int'l/Domestic
        return new AddRoutingResult(AddRoutingOutcome.Created, ListRoutingsQueryHandler<TRouting>.ToResponse(routing));
    }
}
```

(Routing *is* generic over `TRouting : OpsRouting` — arrival vs departure — it just isn't generic over "which sub-resource" the way §3.2 is.)

### 3.4 Cross-cutting: `IFlightCountryTypeResolver`

Shared by both the main flight save path and the routing-leg CRUD above (`AeroFlight.Application/Flights/FlightCountryTypeResolver.cs`):

```csharp
public interface IFlightCountryTypeResolver
{
    Task<string> ResolveAsync(IEnumerable<string?> airportCodes, CancellationToken ct);
}
```

Joins `Airport → Country → CountryType`, groups by distinct `CountryType.Code`, and returns the one with the highest `Severity` ("worst wins" — International > Transborder > Domestic). This is why `Country.CountryTypeCode` (not `Airport`'s own now-removed dead `CountryTypeCode` column) is the actual source of truth for a flight's Int'l/Domestic badge — editing it via **Reference Data → Countries** immediately affects every flight that touches that country on its next save/routing-leg edit.

### 3.5 Audit trail

Two independent mechanisms, both driven by `AuditInterceptor` (`AeroFlight.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs`), an EF `SaveChangesInterceptor` registered globally — no per-handler code writes audit rows.

**Per-entity audit log.** An entity gets audit tracking by calling `.HasAuditType<TEntity, TLog>()` in its EF configuration:

```csharp
// ArrivalGateConfiguration.cs
builder.HasAuditType<OpsArrivalGate, OpsArrivalGateAudit>();
```

`TLog` is a near-empty class (`sealed class OpsArrivalGateAudit : AuditLogBase { }`) mapped to its own table under the `audits` schema by `BaseAuditConfiguration<T>` (`ToTable(typeof(T).Name, "audits")`). `AuditLogBase` carries `Id, PrimaryKey (jsonb), Action, Snapshot (jsonb), Timestamp, ChangedBy`, plus an optional `FlightId` — set automatically for any entity inheriting `FlightResourceBase` (Gate/Belt/Counter), and explicitly by `AuditInterceptor` for `OpsFlightParkPosition` (which uses `ArrivalFlightId`/`DepartureFlightId` instead, so it doesn't inherit `FlightResourceBase`).

On every `SaveChanges`, the interceptor walks `ChangeTracker.Entries()`, and for any tracked entity type present in `AuditLogTypeRegistry` builds an `AuditLogBase` row (`Action` = Added/Modified/Deleted, `Snapshot` = JSON diff of changed properties for Modified, full state for Added/Deleted) and saves it in the *same* `SaveChangesAsync` call, guarded by a re-entrancy flag (`_writingAudit`) so writing the audit rows doesn't recursively trigger more audit rows.

**Merged flight history.** `GetFlightFullAuditQueryHandler` (`AeroFlight.Application/Flights/GetFlightFullAuditQuery.cs`) is what `GET /api/flights/{arrivals|departures}/{id}/audit` actually calls. A flight's own audit log (`ArrivalFlightAudit`/`DepartureFlightAudit`) only covers changes to the flight row itself — Gate/Belt/Counter/Park Position live in their own tables with their own audit logs, keyed by the *resource's* id, not the flight's. This handler merges all of them into one timeline by querying each resource audit table `WHERE FlightId == flightId` (label-prefixed, e.g. `"Gate: Modified"`) alongside the flight's own log, sorted together by timestamp — so a resource that's since been deleted still shows up correctly.

**Adding audit to a new entity type** = 3 files: the `*Audit : AuditLogBase` domain class, its `BaseAuditConfiguration<T>` EF config, and one `.HasAuditType<TEntity, TAudit>()` line in the live entity's own config — plus, if it's a flight sub-resource, one `LoadResourceAsync<TAudit>(...)` call added to `GetFlightFullAuditQueryHandler`.

### 3.6 Business Rules engine (brief)

`IRuleEngine.EvaluateAsync(RuleTrigger, flight, ct)` is called at fixed points in every flight create/update/delete handler (`FlightCreated`/`FlightUpdated`/`FlightDeleted`, plus direction-specific triggers like `ArrivalCreated`). Rules are user-defined (stored `BusinessRule`/`RuleCondition`/`RuleAction` rows, editable via `/rules`), matched against a fixed catalog of condition/action fields (`FlightRuleFieldCatalog`). This is a separate subsystem from both Reference Data and the resource CRUD above — mentioned here only because every flight write handler touches it.

---

## 4. Quick file-location reference

| Concept | Reference Data | Ops |
|---|---|---|
| Domain entity | `AeroFlight.Domain/Metadata/*.cs` | `AeroFlight.Domain/Ops/**/*.cs` |
| EF config | `AeroFlight.Infrastructure/Persistence/Configurations/ReferenceData/*.cs` | `.../Configurations/Flights/**/*.cs` |
| Audit domain class | `AeroFlight.Domain/Audit/MetaData/*.cs` | `AeroFlight.Domain/Audit/Ops/**/*.cs` |
| Audit EF config | `.../Configurations/Audits/MetaData/*.cs` | `.../Configurations/Audits/Flights/**/*.cs` |
| Contracts (Request/Response) | `AeroFlight.Contracts/ReferenceData/*.cs` | `AeroFlight.Contracts/Flights/*.cs` |
| Handlers + validators | `AeroFlight.Application/ReferenceData/*.cs` (generic, per-entity Map/Reader only) | `AeroFlight.Application/Flights/**/*.cs` (per-feature) |
| DI registration | `AeroFlight.Infrastructure/ReferenceData/ReferenceDataServiceCollectionExtensions.cs` | `AeroFlight.Infrastructure/DependencyInjectionService.cs` |
| Endpoint mapping | `AeroFlight.Api/Endpoints/ReferenceData/*.cs` | `AeroFlight.Api/Endpoints/Flights/*.cs`, `.../Resources/*.cs` |
| Seed data | `AeroFlight.Migrations/Seeds/*.csv` + `ResourceSeeder.cs` | (flights are operational data, not seeded) |
| Web catalog / generic screen | `AeroFlight.Web/Services/ReferenceData/ReferenceCatalog.cs`, `Pages/Reference/*.razor` | Hand-built pages per feature, `Pages/Flights/*.razor` |
