using Aero.Flights.Domain.Ops.Resources;

namespace Aero.Flights.Application.Flights.CodeShares;

public interface ICodeShareFactory<TCodeShare> where TCodeShare : OpsCodeShare
{
    TCodeShare Create(long flightId, string airlineCode, string flightNumber);
}

public sealed class ArrivalCodeShareFactory : ICodeShareFactory<OpsArrivalCodeShare>
{
    public OpsArrivalCodeShare Create(long flightId, string airlineCode, string flightNumber) =>
        OpsArrivalCodeShare.Create(flightId, airlineCode, flightNumber);
}

public sealed class DepartureCodeShareFactory : ICodeShareFactory<OpsDepartureCodeShare>
{
    public OpsDepartureCodeShare Create(long flightId, string airlineCode, string flightNumber) =>
        OpsDepartureCodeShare.Create(flightId, airlineCode, flightNumber);
}
