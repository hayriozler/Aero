using AeroFlight.Domain.Ops.Resources;

namespace AeroFlight.Application.Flights.Resources;

public interface IFlightResourceFactory<TResource> where TResource : ResourceBase
{
    TResource Create(long flightId, string? code);
}

public sealed class ArrivalGateResourceFactory : IFlightResourceFactory<OpsArrivalGate>
{
    public OpsArrivalGate Create(long flightId, string? code) => OpsArrivalGate.Create(flightId, code);
}

public sealed class DepartureGateResourceFactory : IFlightResourceFactory<OpsDepartureGate>
{
    public OpsDepartureGate Create(long flightId, string? code) => OpsDepartureGate.Create(flightId, code);
}

public sealed class ArrivalBeltResourceFactory : IFlightResourceFactory<OpsArrivalBelt>
{
    public OpsArrivalBelt Create(long flightId, string? code) => OpsArrivalBelt.Create(flightId, code);
}

public sealed class DepartureBeltResourceFactory : IFlightResourceFactory<OpsDepartureBelt>
{
    public OpsDepartureBelt Create(long flightId, string? code) => OpsDepartureBelt.Create(flightId, code);
}

public sealed class DepartureCounterResourceFactory : IFlightResourceFactory<OpsCounter>
{
    public OpsCounter Create(long flightId, string? code) => OpsCounter.Create(flightId, code);
}
