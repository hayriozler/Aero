using AeroFlight.Domain.Ops.Routing;

namespace AeroFlight.Application.Flights.Routings;

public interface IRoutingFactory<TRouting> where TRouting : OpsRouting
{
    TRouting Create(long flightId, string airportCode, int legNumber);
}

public sealed class ArrivalRoutingFactory : IRoutingFactory<OpsArrivalRouting>
{
    public OpsArrivalRouting Create(long flightId, string airportCode, int legNumber) =>
        OpsArrivalRouting.Create(flightId, airportCode, legNumber);
}

public sealed class DepartureRoutingFactory : IRoutingFactory<OpsDepartureRouting>
{
    public OpsDepartureRouting Create(long flightId, string airportCode, int legNumber) =>
        OpsDepartureRouting.Create(flightId, airportCode, legNumber);
}
