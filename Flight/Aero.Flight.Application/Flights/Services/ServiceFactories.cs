using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops.Services;

namespace Aero.Flights.Application.Flights.Services;

public interface IServiceFactory<TService> where TService : OpsService
{
    TService Create(long flightId, ServiceRequest request);
}

public sealed class ArrivalServiceFactory : IServiceFactory<OpsArrivalService>
{
    public OpsArrivalService Create(long flightId, ServiceRequest request)
    {
        var service = OpsArrivalService.Create(flightId, request.ServiceCode);
        ServiceMapping.Apply(service, request);
        return service;
    }
}

public sealed class DepartureServiceFactory : IServiceFactory<OpsDepartureService>
{
    public OpsDepartureService Create(long flightId, ServiceRequest request)
    {
        var service = OpsDepartureService.Create(flightId, request.ServiceCode);
        ServiceMapping.Apply(service, request);
        return service;
    }
}

internal static class ServiceMapping
{
    public static void Apply(OpsService service, ServiceRequest r)
    {
        service.SetPlan(r.BeginPlan, r.EndPlan, r.AmountPlan);
        service.RecordActual(r.BeginActual, r.EndActual, r.AmountActual);
        service.UpdateHandlingAgent(r.HandlingAgentCode);
        service.UpdateComment(r.Comment);
        service.UpdatePayer(r.PayerCompanyCode);
    }
}
