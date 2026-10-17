using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Queries;

public sealed class GetDepartureFlightDetailQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetDepartureFlightDetailQuery, DepartureFlightDetail?>
{
    public async Task<DepartureFlightDetail?> HandleAsync(GetDepartureFlightDetailQuery query, CancellationToken ct)
    {
        var f = await db.Query<DepartureFlight>().Include(x => x.Routings).FirstOrDefaultAsync(x => x.Id == query.Id, ct);
        if (f is null) return null;

        return new DepartureFlightDetail(
            f.Id, f.AirlineCode, f.FlightNumber, f.CallSign, f.CodeshareNumber, f.FlightTypeCode,
            f.Status, f.AircraftCode, f.AircraftTypeCode, f.OperatingAirlineCode, f.TerminalCode,
            f.HandlingAgentCode, f.Comment,
            f.Gates, f.GateStatusCode, f.ParkPositionCode, f.CurrentParkPositionCode,
            f.IsGeneralAviation,
            f.CancellationDateTime, f.LoadInfo.PaxTotal,
            f.ArrivalFlightId, f.Std, f.Etd, f.Atd, f.OffBlock, f.TakeOff, f.FlightDate,
            f.DestinationAirportCode, f.RoutingSummary, f.Counters, f.CounterStatusCode,
            f.GateBegin, f.GateEnd, f.CountryType,
            new FlightLoadInfoResponse(
                f.LoadInfo.PaxTransit, f.LoadInfo.PaxAssistanceRequired, f.LoadInfo.Pax1, f.LoadInfo.Pax2, f.LoadInfo.Pax3,
                f.LoadInfo.CrewNumber, f.LoadInfo.CrewCockpit,
                f.LoadInfo.BaggageWeight, f.LoadInfo.BaggageTotal, f.LoadInfo.CargoWeight, f.LoadInfo.MailWeight,
                f.LoadInfo.TotalWeight, f.LoadInfo.EquipmentWeight, f.LoadInfo.CabinLuggage));
    }
}
