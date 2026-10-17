using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.ParkPositiions;

public sealed record ListParkPositionsQuery(long FlightId, bool IsArrival);
public sealed record AddParkPositionCommand(long FlightId, bool IsArrival, OpsParkPositionRequest Request);
public sealed record UpdateParkPositionCommand(long FlightId, bool IsArrival, long ParkPositionId, OpsParkPositionRequest Request);
public sealed record DeleteParkPositionCommand(long FlightId, bool IsArrival, long ParkPositionId);

public sealed class ListParkPositionsQueryHandler(IAeroFlightDbContext db) : IQueryHandler<ListParkPositionsQuery, IReadOnlyList<OpsParkPositionResponse>>
{
    public async Task<IReadOnlyList<OpsParkPositionResponse>> HandleAsync(ListParkPositionsQuery query, CancellationToken ct)
    {
        var items = query.IsArrival
            ? await db.Query<OpsFlightParkPosition>().Where(s => s.ArrivalFlightId == query.FlightId).OrderBy(s => s.BeginPlan).ToListAsync(ct)
            : await db.Query<OpsFlightParkPosition>().Where(s => s.DepartureFlightId == query.FlightId).OrderBy(s => s.BeginPlan).ToListAsync(ct);

        return [.. items.Select(ToResponse)];
    }

    internal static OpsParkPositionResponse ToResponse(OpsFlightParkPosition s) => new(
        s.Id, s.ArrivalFlightId, s.DepartureFlightId, s.Code, s.LastCode,
        s.BeginPlan, s.EndPlan, s.BeginActual, s.EndActual, s.PayerCompanyCode);
}

public sealed class AddParkPositionCommandHandler(IAeroFlightDbContext db) : ICommandHandler<AddParkPositionCommand, OpsParkPositionResponse>
{
    public async Task<OpsParkPositionResponse> HandleAsync(AddParkPositionCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var parkPosition = command.IsArrival
            ? OpsFlightParkPosition.CreateForArrival(command.FlightId, r.BeginPlan, r.EndPlan)
            : OpsFlightParkPosition.CreateForDeparture(command.FlightId, r.BeginPlan, r.EndPlan);

        parkPosition.AssignParkPosition(r.Code);
        parkPosition.UpdatePayer(r.PayerCompanyCode);

        db.Add(parkPosition);
        await db.SaveChangesAsync(ct);
        return ListParkPositionsQueryHandler.ToResponse(parkPosition);
    }
}

public sealed class UpdateParkPositionCommandHandler(IAeroFlightDbContext db) : ICommandHandler<UpdateParkPositionCommand, OpsParkPositionResponse?>
{
    public async Task<OpsParkPositionResponse?> HandleAsync(UpdateParkPositionCommand command, CancellationToken ct)
    {
        var parkPosition = await db.Query<OpsFlightParkPosition>().FirstOrDefaultAsync(s => s.Id == command.ParkPositionId
            && (command.IsArrival ? s.ArrivalFlightId == command.FlightId : s.DepartureFlightId == command.FlightId), ct);
        if (parkPosition is null)
            return null;

        var r = command.Request;
        if (!string.Equals(parkPosition.Code, r.Code, StringComparison.Ordinal))
            parkPosition.AssignParkPosition(r.Code);
        parkPosition.UpdatePlan(r.BeginPlan, r.EndPlan);
        if (r.BeginActual is { } beginActual)
            parkPosition.RecordActual(beginActual, r.EndActual);
        parkPosition.UpdatePayer(r.PayerCompanyCode);

        await db.SaveChangesAsync(ct);
        return ListParkPositionsQueryHandler.ToResponse(parkPosition);
    }
}

public sealed class DeleteParkPositionCommandHandler(IAeroFlightDbContext db) : ICommandHandler<DeleteParkPositionCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteParkPositionCommand command, CancellationToken ct)
    {
        var parkPosition = await db.Query<OpsFlightParkPosition>().FirstOrDefaultAsync(s => s.Id == command.ParkPositionId
            && (command.IsArrival ? s.ArrivalFlightId == command.FlightId : s.DepartureFlightId == command.FlightId), ct);
        if (parkPosition is null)
            return false;

        db.Remove(parkPosition);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
