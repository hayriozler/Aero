using Aero.Scanner.Api.Persistence;
using Aero.Scanner.Domain.Aggregates.Mappings;
using Aero.Scanner.Shared.Consts;
using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;
using Aero.Shared.WebUtilities.Validations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Aero.Scanner.Api.Endpoints;

public static class ResourceStatusMapEndpoints
{
    public static IEndpointRouteBuilder MapResourceStatusMapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/resource-status-maps")
            .RequireAuthorization(Policies.AirportOperator);

        group.MapGet("/", GetAll)
            .WithName("GetResourceStatusMaps")
            .Produces<IReadOnlyList<ResourceStatusMapDto>>();

        group.MapPost("/", Create)
            .WithName("CreateResourceStatusMap")
            .AddEndpointFilter<EndPointValidationFilter<CreateResourceStatusMapRequest>>()
            .Produces<ResourceStatusMapDto>(201)
            .ProducesProblem(409);

        group.MapPut("/{sourceName}/{sourceStatus}", Update)
            .WithName("UpdateResourceStatusMap")
            .AddEndpointFilter<EndPointValidationFilter<UpdateResourceStatusMapRequest>>()
            .Produces<ResourceStatusMapDto>()
            .ProducesProblem(404);

        group.MapDelete("/{sourceName}/{sourceStatus}", Delete)
            .WithName("DeleteResourceStatusMap")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<ResourceStatusMapDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var items = await db.ResourceStatusMapSet
            .AsNoTracking()
            .OrderBy(m => m.SourceResourceName).ThenBy(m => m.SourceResourceStatus)
            .Select(m => ToDto(m))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<ResourceStatusMapDto>>(items);
    }

    private static async Task<Results<Created<ResourceStatusMapDto>, Conflict<string>>> Create(
        CreateResourceStatusMapRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var exists = await db.ResourceStatusMapSet.AnyAsync(
            m => m.SourceResourceName   == request.SourceResourceName.Trim()
              && m.SourceResourceStatus == request.SourceResourceStatus.Trim(), ct);

        if (exists)
            return TypedResults.Conflict("A mapping for this resource name and status already exists.");

        var map = ResourceStatusMap.Create(
            request.SourceResourceName,
            request.SourceResourceStatus,
            request.TargetResourceStatus);

        db.ResourceStatusMapSet.Add(map);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created(
            $"/api/resource-status-maps/{map.SourceResourceName}/{map.SourceResourceStatus}",
            ToDto(map));
    }

    private static async Task<Results<Ok<ResourceStatusMapDto>, NotFound>> Update(
        string sourceName, string sourceStatus, UpdateResourceStatusMapRequest request,
        AeroScanDataContext db, CancellationToken ct)
    {
        var map = await db.ResourceStatusMapSet.FirstOrDefaultAsync(
            m => m.SourceResourceName   == sourceName
              && m.SourceResourceStatus == sourceStatus, ct);

        if (map is null) return TypedResults.NotFound();

        map.Update(request.TargetResourceStatus);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(map));
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        string sourceName, string sourceStatus, AeroScanDataContext db, CancellationToken ct)
    {
        var map = await db.ResourceStatusMapSet.FirstOrDefaultAsync(
            m => m.SourceResourceName   == sourceName
              && m.SourceResourceStatus == sourceStatus, ct);

        if (map is null) return TypedResults.NotFound();

        db.ResourceStatusMapSet.Remove(map);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static ResourceStatusMapDto ToDto(ResourceStatusMap m) => new(
        m.SourceResourceName, m.SourceResourceStatus, m.TargetResourceStatus);
}
