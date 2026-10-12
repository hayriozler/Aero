using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Flights;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class ContainerEndpoints
{
    public static IEndpointRouteBuilder MapContainerEndpoints(this IEndpointRouteBuilder app)
    {
        // Read endpoints — any authenticated user (handling agents, ramp supervisors, etc.)
        var readGroup = app.MapGroup("/api/containers")
            .RequireAuthorization();

        readGroup.MapGet("/", GetByFlight)
            .WithName("GetContainersByFlight")
            .Produces<IReadOnlyList<ContainerDto>>()
            .ProducesProblem(400);

        readGroup.MapGet("/{id:int}", GetById)
            .WithName("GetContainerById")
            .Produces<ContainerDto>()
            .ProducesProblem(404);

        // Create — airport operator or ramp supervisor (handling agent)
        var createGroup = app.MapGroup("/api/containers")
            .RequireAuthorization(Policies.AirportOrRamp);

        createGroup.MapPost("/", Create)
            .WithName("CreateContainer")
            .AddEndpointFilter<ValidationFilter<CreateContainerRequest>>()
            .Produces<ContainerDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(404);

        // Modify/delete — airport operator only
        var editGroup = app.MapGroup("/api/containers")
            .RequireAuthorization(Policies.AirportOperator);

        editGroup.MapPut("/{id:int}", Update)
            .WithName("UpdateContainer")
            .AddEndpointFilter<ValidationFilter<UpdateContainerRequest>>()
            .Produces<ContainerDto>()
            .ProducesProblem(400)
            .ProducesProblem(404);

        editGroup.MapDelete("/{id:int}", Delete)
            .WithName("DeleteContainer")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Results<Ok<IReadOnlyList<ContainerDto>>, BadRequest<string>>> GetByFlight(
        int flightId, AeroScanDataContext db, CancellationToken ct)
    {
        if (flightId <= 0)
            return TypedResults.BadRequest("flightId is required.");

        var containers = await db.ContainerSet
            .AsNoTracking()
            .Where(c => c.FlightId == flightId)
            .OrderBy(c => c.ContainerCode)
            .Select(c => ToDto(c))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<ContainerDto>>(containers);
    }

    private static async Task<Results<Ok<ContainerDto>, NotFound>> GetById(
        int id, AeroScanDataContext db, CancellationToken ct)
    {
        var c = await db.ContainerSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return c is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(c));
    }

    private static async Task<Results<Created<ContainerDto>, BadRequest<string>, NotFound>> Create(
        CreateContainerRequest request, AeroScanDataContext db, HttpContext ctx, CancellationToken ct)
    {
        var flightExists = await db.DepartureFlightSet.AnyAsync(f => f.Id == request.FlightId, ct);
        if (!flightExists)
            return TypedResults.NotFound();

        var username = ctx.User.FindFirst("unique_name")?.Value ?? "system";
        var container = Container.Create(
            request.FlightId,
            request.ContainerCode,
            request.ContainerTypeCode,
            request.ContainerStatusCode,
            request.ContainerClassCode,
            request.ContainerDestination);

        db.ContainerSet.Add(container);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/containers/{container.Id}", ToDto(container));
    }

    private static async Task<Results<Ok<ContainerDto>, BadRequest<string>, NotFound>> Update(
        int id, UpdateContainerRequest request, AeroScanDataContext db, HttpContext ctx, CancellationToken ct)
    {
        var container = await db.ContainerSet.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (container is null) return TypedResults.NotFound();

        var username = ctx.User.FindFirst("unique_name")?.Value ?? "system";
        container.Update(
            request.ContainerCode,
            request.ContainerTypeCode,
            request.ContainerStatusCode,
            request.ContainerClassCode,
            request.ContainerDestination);

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(container));
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        int id, AeroScanDataContext db, CancellationToken ct)
    {
        var container = await db.ContainerSet.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (container is null) return TypedResults.NotFound();

        db.ContainerSet.Remove(container);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static ContainerDto ToDto(Container c) => new(
        c.Id, c.FlightId, c.ContainerCode, c.ContainerTypeCode,
        c.ContainerStatusCode, c.ContainerClassCode, c.ContainerDestination);
}
