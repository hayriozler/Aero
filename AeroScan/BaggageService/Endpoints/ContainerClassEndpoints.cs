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

public static class ContainerClassEndpoints
{
    public static IEndpointRouteBuilder MapContainerClassEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/container-classes")
            .RequireAuthorization(Policies.AirportOperator);

        group.MapGet("/", GetAll)
            .WithName("GetAllContainerClasses")
            .Produces<IReadOnlyList<ContainerClassDto>>();

        group.MapGet("/{typeCode}", GetByTypeCode)
            .WithName("GetContainerClassByTypeCode")
            .Produces<ContainerClassDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreateContainerClass")
            .AddEndpointFilter<ValidationFilter<CreateContainerClassRequest>>()
            .Produces<ContainerClassDto>(201)
            .ProducesProblem(409);

        group.MapPut("/{typeCode}", Update)
            .WithName("UpdateContainerClass")
            .AddEndpointFilter<ValidationFilter<UpdateContainerClassRequest>>()
            .Produces<ContainerClassDto>()
            .ProducesProblem(404);

        group.MapDelete("/{typeCode}", Delete)
            .WithName("DeleteContainerClass")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<ContainerClassDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var items = await db.ContainerTypeClassSet
            .AsNoTracking()
            .OrderBy(c => c.TypeCode)
            .Select(c => ToDto(c))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<ContainerClassDto>>(items);
    }

    private static async Task<Results<Ok<ContainerClassDto>, NotFound>> GetByTypeCode(
        string typeCode, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.ContainerTypeClassSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.TypeCode == typeCode.ToUpperInvariant(), ct);

        return item is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(item));
    }

    private static async Task<Results<Created<ContainerClassDto>, Conflict<string>>> Create(
        CreateContainerClassRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var exists = await db.ContainerTypeClassSet
            .AnyAsync(c => c.TypeCode == request.ContainerTypeCode.ToUpperInvariant(), ct);

        if (exists)
            return TypedResults.Conflict($"Container class for type '{request.ContainerTypeCode}' already exists.");

        var item = ContainerTypeClass.Create(request.ContainerTypeCode, request.ClassCode, request.Description);
        db.ContainerTypeClassSet.Add(item);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/container-classes/{item.TypeCode}", ToDto(item));
    }

    private static async Task<Results<Ok<ContainerClassDto>, NotFound>> Update(
        string typeCode, UpdateContainerClassRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.ContainerTypeClassSet
            .FirstOrDefaultAsync(c => c.TypeCode == typeCode.ToUpperInvariant(), ct);

        if (item is null) return TypedResults.NotFound();

        item.Update(request.Description);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(item));
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        string typeCode, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.ContainerTypeClassSet
            .FirstOrDefaultAsync(c => c.TypeCode == typeCode.ToUpperInvariant(), ct);

        if (item is null) return TypedResults.NotFound();

        db.ContainerTypeClassSet.Remove(item);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static ContainerClassDto ToDto(ContainerTypeClass c) =>
        new(c.TypeCode, c.ClassCode, c.Description);
}
