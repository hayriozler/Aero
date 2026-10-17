using Aero.Flights.Application.Flights.Resources;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aero.Flights.Application.Flights.InboundProcessors;

public abstract class FlightInboundProcessor<TMessage>(IAeroFlightDbContext db) : IInboundMessageProcessor
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected IAeroFlightDbContext Db => db;

    public abstract string MessageType { get; }

    public async Task ProcessAsync(string payload, CancellationToken ct)
    {
        var message = JsonSerializer.Deserialize<TMessage>(payload, JsonOptions);
        if (message is not null)
            await ApplyAsync(message, ct);
    }

    protected abstract Task ApplyAsync(TMessage message, CancellationToken ct);

    protected async Task<ArrivalFlight> FindArrivalAsync(long? id, string flightNumber, DateTimeOffset sta, CancellationToken ct)
    {
        if (id is { } flightId)
            return await Db.Query<ArrivalFlight>().FirstOrDefaultAsync(f => f.Id == flightId, ct)
                ?? throw new InvalidOperationException($"No arrival flight found for id {flightId}.");

        var flightDate = DateOnly.FromDateTime(sta.UtcDateTime.Date);
        return await Db.Query<ArrivalFlight>().FirstOrDefaultAsync(f => f.FlightNumber == flightNumber && f.FlightDate == flightDate, ct)
            ?? throw new InvalidOperationException($"No arrival flight found for '{flightNumber}' on {flightDate:O}.");
    }

    protected async Task<DepartureFlight> FindDepartureAsync(long? id, string flightNumber, DateTimeOffset std, CancellationToken ct)
    {
        if (id is { } flightId)
            return await Db.Query<DepartureFlight>().FirstOrDefaultAsync(f => f.Id == flightId, ct)
                ?? throw new InvalidOperationException($"No departure flight found for id {flightId}.");

        var flightDate = DateOnly.FromDateTime(std.UtcDateTime.Date);
        return await Db.Query<DepartureFlight>().FirstOrDefaultAsync(f => f.FlightNumber == flightNumber && f.FlightDate == flightDate, ct)
            ?? throw new InvalidOperationException($"No departure flight found for '{flightNumber}' on {flightDate:O}.");
    }

    protected async Task ApplyResourceMessageAsync<TResource>(
        IFlightResourceService<TResource> resourceService, long flightId, string resourceKind,
        IReadOnlyList<FlightResourceOperation> operations, CancellationToken ct)
        where TResource : ResourceBase
    {
        foreach (var op in operations)
        {
            switch (op.Operation)
            {
                case "assign":
                    await resourceService.CreateAsync(flightId, ToRequest(op), ct);
                    break;
                case "update":
                    await resourceService.PatchAsync(await FindResourceAsync<TResource>(flightId, resourceKind, op, ct), op, ct);
                    break;
                case "release":
                    await resourceService.DeleteAsync(await FindResourceAsync<TResource>(flightId, resourceKind, op, ct), ct);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported resource operation '{op.Operation}'.");
            }
        }
    }

    private static FlightResourceRequest ToRequest(FlightResourceOperation op) =>
        new(op.Code, op.BeginPlan, op.EndPlan, op.StatusCode, op.BeginActual, op.EndActual);

    private async Task<TResource> FindResourceAsync<TResource>(
        long flightId, string resourceKind, FlightResourceOperation op, CancellationToken ct)
        where TResource : ResourceBase
    {
        if (op.ResourceId is { } resourceId)
            return await Db.Query<TResource>().FirstOrDefaultAsync(r => r.Id == resourceId, ct)
                ?? throw new InvalidOperationException($"No {resourceKind} resource found for id {resourceId}.");

        return await Db.Query<TResource>().FirstOrDefaultAsync(r => r.FlightId == flightId && r.Code == op.Code, ct)
            ?? throw new InvalidOperationException($"No {resourceKind} resource found for flight {flightId} code '{op.Code}'.");
    }
}
