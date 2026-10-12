using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Containers;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class ContainerTypeEndpoints
{
    public static IEndpointRouteBuilder MapContainerTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/container-types")
            .RequireAuthorization(Policies.AirportOperator);

        group.MapGet("/", GetAll)
            .WithName("GetAllContainerTypes")
            .Produces<IReadOnlyList<ContainerTypeDto>>();

        group.MapGet("/{code}", GetByCode)
            .WithName("GetContainerTypeByCode")
            .Produces<ContainerTypeDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreateContainerType")
            .AddEndpointFilter<ValidationFilter<CreateContainerTypeRequest>>()
            .Produces<ContainerTypeDto>(201)
            .ProducesProblem(409);

        group.MapPut("/{code}", Update)
            .WithName("UpdateContainerType")
            .AddEndpointFilter<ValidationFilter<UpdateContainerTypeRequest>>()
            .Produces<ContainerTypeDto>()
            .ProducesProblem(404);

        group.MapDelete("/{code}", Delete)
            .WithName("DeleteContainerType")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<ContainerTypeDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var items = await db.ContainerTypeSet
            .AsNoTracking()
            .OrderBy(t => t.Code)
            .Select(t => ToDto(t))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<ContainerTypeDto>>(items);
    }

    private static async Task<Results<Ok<ContainerTypeDto>, NotFound>> GetByCode(
        string code, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.ContainerTypeSet.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Code == code.ToUpperInvariant(), ct);

        return item is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(item));
    }

    private static async Task<Results<Created<ContainerTypeDto>, Conflict<string>>> Create(
        CreateContainerTypeRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var exists = await db.ContainerTypeSet.AnyAsync(t => t.Code == request.Code.ToUpperInvariant(), ct);
        if (exists)
            return TypedResults.Conflict($"Container type '{request.Code}' already exists.");

        var item = ContainerType.Create(request.Code, request.Description, request.IsAllDestination, request.IsTransfer);
        db.ContainerTypeSet.Add(item);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/container-types/{item.Code}", ToDto(item));
    }

    private static async Task<Results<Ok<ContainerTypeDto>, NotFound>> Update(
        string code, UpdateContainerTypeRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.ContainerTypeSet
            .FirstOrDefaultAsync(t => t.Code == code.ToUpperInvariant(), ct);

        if (item is null) return TypedResults.NotFound();

        item.Update(request.Description, request.IsAllDestination, request.IsTransfer);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(item));
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        string code, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.ContainerTypeSet
            .FirstOrDefaultAsync(t => t.Code == code.ToUpperInvariant(), ct);

        if (item is null) return TypedResults.NotFound();

        db.ContainerTypeSet.Remove(item);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static ContainerTypeDto ToDto(ContainerType t) =>
        new(t.Code, t.Description, t.IsAllDestination, t.IsTransfer);
}
