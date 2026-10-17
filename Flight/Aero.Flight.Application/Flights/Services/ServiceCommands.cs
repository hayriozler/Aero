using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops.Services;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Services;

public sealed record ListServicesQuery<TService>(long FlightId) where TService : OpsService;
public sealed record AddServiceCommand<TService>(long FlightId, ServiceRequest Request) where TService : OpsService;
public sealed record UpdateServiceCommand<TService>(long FlightId, long ServiceId, ServiceRequest Request) where TService : OpsService;
public sealed record DeleteServiceCommand<TService>(long FlightId, long ServiceId) where TService : OpsService;

public sealed class ListServicesQueryHandler<TService>(IAeroFlightDbContext db)
    : IQueryHandler<ListServicesQuery<TService>, IReadOnlyList<ServiceResponse>>
    where TService : OpsService
{
    public async Task<IReadOnlyList<ServiceResponse>> HandleAsync(ListServicesQuery<TService> query, CancellationToken ct)
    {
        var items = await db.Query<TService>()
            .Where(s => s.FlightId == query.FlightId)
            .OrderBy(s => s.Id)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static ServiceResponse ToResponse(TService s) => new(
        s.Id, s.FlightId, s.ServiceCode,
        s.BeginPlan, s.EndPlan, s.AmountPlan,
        s.BeginActual, s.EndActual, s.AmountActual,
        s.HandlingAgentCode, s.Comment, s.PayerCompanyCode);
}

public sealed class AddServiceCommandHandler<TService>(IAeroFlightDbContext db, IServiceFactory<TService> factory)
    : ICommandHandler<AddServiceCommand<TService>, ServiceResponse>
    where TService : OpsService
{
    public async Task<ServiceResponse> HandleAsync(AddServiceCommand<TService> command, CancellationToken ct)
    {
        var service = factory.Create(command.FlightId, command.Request);
        db.Add(service);
        await db.SaveChangesAsync(ct);
        return ListServicesQueryHandler<TService>.ToResponse(service);
    }
}

public sealed class UpdateServiceCommandHandler<TService>(IAeroFlightDbContext db)
    : ICommandHandler<UpdateServiceCommand<TService>, ServiceResponse?>
    where TService : OpsService
{
    public async Task<ServiceResponse?> HandleAsync(UpdateServiceCommand<TService> command, CancellationToken ct)
    {
        var service = await db.Query<TService>().FirstOrDefaultAsync(s => s.Id == command.ServiceId && s.FlightId == command.FlightId, ct);
        if (service is null)
            return null;

        var r = command.Request;
        service.ChangeServiceCode(r.ServiceCode);
        ServiceMapping.Apply(service, r);
        await db.SaveChangesAsync(ct);
        return ListServicesQueryHandler<TService>.ToResponse(service);
    }
}

public sealed class DeleteServiceCommandHandler<TService>(IAeroFlightDbContext db)
    : ICommandHandler<DeleteServiceCommand<TService>, bool>
    where TService : OpsService
{
    public async Task<bool> HandleAsync(DeleteServiceCommand<TService> command, CancellationToken ct)
    {
        var service = await db.Query<TService>().FirstOrDefaultAsync(s => s.Id == command.ServiceId && s.FlightId == command.FlightId, ct);
        if (service is null)
            return false;

        db.Remove(service);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
