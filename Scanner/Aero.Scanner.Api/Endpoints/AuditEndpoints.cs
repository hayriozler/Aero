using Aero.Scanner.Api.Persistence;
using Aero.Scanner.Shared.Consts;
using Aero.Scanner.Shared.Dtos;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Aero.Scanner.Api.Endpoints;

public static class AuditEndpoints
{
    private static readonly Lazy<Dictionary<string, Type>> _logTypesLazy =
        new(() => AuditLogTypeRegistry.All
            .ToDictionary(
                kv => kv.Key.Name,
                kv => kv.Value,
                StringComparer.OrdinalIgnoreCase),
            LazyThreadSafetyMode.ExecutionAndPublication);

    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/audits")
            .RequireAuthorization(Policies.AnyRole);

        group.MapGet("/{entityType}", GetHistory)
            .WithName("GetAuditHistory")
            .Produces<IReadOnlyList<AuditEntryDto>>()
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Results<Ok<IReadOnlyList<AuditEntryDto>>, NotFound<string>>> GetHistory(
        string entityType,
        string pk,
        AeroScanDataContext db,
        CancellationToken ct)
    {
        if (!_logTypesLazy.Value.TryGetValue(entityType, out var logType))
            return TypedResults.NotFound($"No audit log configured for entity '{entityType}'.");

        var setMethod = typeof(DbContext)
            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(logType);

        var queryable = (IQueryable<AuditLogBase>)setMethod.Invoke(db, null)!;

        var entries = await queryable
            .AsNoTracking()
            .Where(l => l.PrimaryKey == pk)
            .OrderByDescending(l => l.Timestamp)
            .Select(l => new AuditEntryDto(l.Id, l.Action, l.Snapshot, l.Timestamp, l.CreatedBy))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<AuditEntryDto>>(entries);
    }
}
