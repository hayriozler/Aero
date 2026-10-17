---
name: add-endpoint
description: Scaffold a new Minimal API endpoint group following the AeroFlight pattern (ICommandHandler/IQueryHandler, ApiResult wrapper, FluentValidation, standard response envelope).
---

# Add Endpoint

Add a new Minimal API endpoint group to `AeroFlight.Api`. Follow **all** conventions below exactly — no exceptions.

## Standard response envelope

Every endpoint must return `ApiResult.*` methods from `AeroFlight.Api.Infrastructure`. Never return raw `Results.Ok`, `Results.NotFound`, etc.

```
{
  requestStartTime, requestFinishTime, httpStatus, traceId,
  data,          // null on error
  pagination,    // null unless paged list
  errors         // null on success; array of { code, message, field }
}
```

`RequestTimingMiddleware` stamps the start time — it is already registered in `Program.cs`.

## File locations

| What | Where |
|---|---|
| Endpoint file | `src/AeroFlight.Api/Endpoints/<Feature>/` |
| Commands/Queries | `src/AeroFlight.Application/<Feature>/` |
| Validators | Same file as the command (inner class) |
| Handlers | Same file as the command/query |
| Infrastructure services | `src/AeroFlight.Infrastructure/<Feature>/` |

## Step-by-step

### 1. Application layer — commands/queries

For each operation create in `src/AeroFlight.Application/<Feature>/`:

```csharp
// GetXxxHandler.cs
public record GetXxxQuery(int Id);

public sealed class GetXxxQueryHandler(IAppDbContext db) : IQueryHandler<GetXxxQuery, XxxResponse?>
{
    public async Task<XxxResponse?> HandleAsync(GetXxxQuery query, CancellationToken ct)
    {
        var entity = await db.XxxSet.FirstOrDefaultAsync(x => x.Id == query.Id, ct);
        return entity is null ? null : XxxResponse.From(entity);
    }
}
```

```csharp
// CreateXxxHandler.cs
public record CreateXxxCommand(string Name /*, ... */);

public sealed class CreateXxxCommandValidator : AbstractValidator<CreateXxxCommand>
{
    public CreateXxxCommandValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(100); }
}

public sealed class CreateXxxCommandHandler(IAppDbContext db) : ICommandHandler<CreateXxxCommand, XxxResponse>
{
    public async Task<XxxResponse> HandleAsync(CreateXxxCommand command, CancellationToken ct)
    {
        var entity = Xxx.Create(command.Name);
        db.XxxSet.Add(entity);
        await db.SaveChangesAsync(ct);
        return XxxResponse.From(entity);
    }
}
```

**Read-only entities** (no `Create`/`Update` domain methods) → GET-only handlers only.

Response record:
```csharp
public record XxxResponse(int Id, string Name, ...)
{
    internal static XxxResponse From(Xxx entity) => new(entity.Id, entity.Name, ...);
}
```

### 2. Register handlers in DependencyInjectionService

Add to `src/AeroFlight.Infrastructure/DependencyInjectionService.cs`:

```csharp
// Queries
services.AddScoped<IQueryHandler<GetXxxsQuery, IReadOnlyList<XxxResponse>>, GetXxxsQueryHandler>();
services.AddScoped<IQueryHandler<GetXxxQuery, XxxResponse?>, GetXxxQueryHandler>();

// Commands (omit if read-only entity)
services.AddScoped<ICommandHandler<CreateXxxCommand, XxxResponse>, CreateXxxCommandHandler>();
services.AddScoped<ICommandHandler<UpdateXxxCommand, XxxResponse?>, UpdateXxxCommandHandler>();
```

### 3. Add DbSet to IAppDbContext (if new entity)

`src/AeroFlight.Application/Interfaces/IAppDbContext.cs`:
```csharp
DbSet<Xxx> XxxSet { get; }
```

### 4. Endpoint file

`src/AeroFlight.Api/Endpoints/<Feature>/XxxEndpoints.cs`:

```csharp
using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.<Feature>;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.<Feature>;

public static class XxxEndpoints
{
    public static IEndpointRouteBuilder MapXxxEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/xxxs").WithTags("Xxxs").RequireAuthorization();

        // GET list
        group.MapGet("/", async (
            IQueryHandler<GetXxxsQuery, IReadOnlyList<XxxResponse>> handler,
            HttpContext ctx, CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetXxxsQuery(), ct)));

        // GET single
        group.MapGet("/{id:int}", async (
            int id,
            IQueryHandler<GetXxxQuery, XxxResponse?> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetXxxQuery(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "XXX_NOT_FOUND", $"Xxx {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        // POST (omit for read-only entities)
        group.MapPost("/", async (
            CreateXxxCommand command,
            ICommandHandler<CreateXxxCommand, XxxResponse> handler,
            IValidator<CreateXxxCommand> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command, ct);
            return ApiResult.Created(ctx, result, $"/api/xxxs/{result.Id}");
        });

        // PUT (omit for read-only entities)
        group.MapPut("/{id:int}", async (
            int id,
            UpdateXxxCommand command,
            ICommandHandler<UpdateXxxCommand, XxxResponse?> handler,
            IValidator<UpdateXxxCommand> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command with { Id = id }, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command with { Id = id }, ct);
            return result is null
                ? ApiResult.NotFound(ctx, "XXX_NOT_FOUND", $"Xxx {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        return app;
    }
}
```

### 5. Register endpoint group in Program.cs

```csharp
app.MapXxxEndpoints();
```

### 6. Paged lists

When the list can be large, use `ApiResult.Paged`:

```csharp
// Query: add Page + PageSize params
public record GetXxxsQuery(int Page = 1, int PageSize = 20);

// Handler:
var total = await db.XxxSet.CountAsync(ct);
var items = await db.XxxSet
    .OrderBy(x => x.Name)
    .Skip((query.Page - 1) * query.PageSize)
    .Take(query.PageSize)
    .ToListAsync(ct);

var pagination = new ApiPagination(query.Page, query.PageSize, total,
    (int)Math.Ceiling((double)total / query.PageSize));

// Endpoint:
return ApiResult.Paged(ctx, items.Select(XxxResponse.From).ToList(), pagination);
```

### 7. Conflict / business errors

```csharp
// In handler: throw
throw new InvalidOperationException("NAME_DUPLICATE|Name already exists.");

// In endpoint: catch and map
try { ... }
catch (InvalidOperationException ex) when (ex.Message.Contains('|'))
{
    var parts = ex.Message.Split('|', 2);
    return ApiResult.Conflict(ctx, parts[0], parts[1]);
}
```

Or check in the endpoint directly after calling the handler if the handler returns a discriminated result.

## ApiResult methods reference

| Method | HTTP | Use when |
|---|---|---|
| `ApiResult.Ok(ctx, data)` | 200 | Successful GET or PUT |
| `ApiResult.Created(ctx, data, location)` | 201 | Successful POST |
| `ApiResult.NoContent(ctx)` | 204 | DELETE / side-effect-only POST |
| `ApiResult.Paged(ctx, data, pagination)` | 200 | Paged list |
| `ApiResult.NotFound(ctx, code, msg)` | 404 | Entity not found |
| `ApiResult.Conflict(ctx, code, msg)` | 409 | Duplicate / business rule |
| `ApiResult.ValidationFailed(ctx, result)` | 422 | FluentValidation failure |
| `ApiResult.Unauthorized(ctx)` | 401 | Bad credentials |

## Error codes convention

Use SCREAMING_SNAKE_CASE. Include the entity name: `USER_NOT_FOUND`, `ROLE_DUPLICATE`, `FLIGHT_INVALID_STATUS`. Validation errors always use code `VALIDATION_ERROR` (set automatically by `ApiResult.ValidationFailed`).

## What NOT to do

- Never return `Results.Ok`, `Results.NotFound`, `Results.ValidationProblem`, etc.
- Never skip `IValidator` for commands that have a validator registered.
- Never put business logic in endpoints — delegate to handlers.
- Never expose EF entities directly — always project to a `*Response` record.
- Read-only entities (no `Create`/`Update` domain methods) get GET endpoints only.
