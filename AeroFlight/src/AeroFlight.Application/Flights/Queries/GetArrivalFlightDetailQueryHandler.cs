using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Ops;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Queries;

public sealed class GetArrivalFlightDetailQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetArrivalFlightDetailQuery, ArrivalFlightDetail?>
{
    public async Task<ArrivalFlightDetail?> HandleAsync(GetArrivalFlightDetailQuery query, CancellationToken ct)
    {
        var f = await db.Query<ArrivalFlight>().Include(x => x.Routings).FirstOrDefaultAsync(x => x.Id == query.Id, ct);
        if (f is null) return null;

        return new ArrivalFlightDetail(
            f.Id, f.AirlineCode, f.FlightNumber, f.CallSign, f.CodeshareNumber, f.FlightTypeCode,
            f.Status, f.AircraftCode, f.AircraftTypeCode, f.OperatingAirlineCode, f.TerminalCode,
            f.HandlingAgentCode, f.Comment,
            f.Gates, f.GateStatusCode, f.Belts, f.BeltStatusCode, f.ParkPositionCode, f.CurrentParkPositionCode,
            f.IsGeneralAviation,
            f.CancellationDateTime, f.LoadInfo.PaxTotal,
            f.DepartureFlightId, f.Sta, f.Eta, f.Ata, f.TouchDown, f.OnBlock,  f.FlightDate,
            f.OriginAirportCode, f.DivertAirportCode, f.RoutingSummary,
            null, f.DeboardingBegin, f.DeboardingEnd, f.CountryType,
            new FlightLoadInfoResponse(
                f.LoadInfo.PaxTransit, f.LoadInfo.PaxAssistanceRequired, f.LoadInfo.Pax1, f.LoadInfo.Pax2, f.LoadInfo.Pax3,
                f.LoadInfo.CrewNumber, f.LoadInfo.CrewCockpit,
                f.LoadInfo.BaggageWeight, f.LoadInfo.BaggageTotal, f.LoadInfo.CargoWeight, f.LoadInfo.MailWeight,
                f.LoadInfo.TotalWeight, f.LoadInfo.EquipmentWeight, f.LoadInfo.CabinLuggage));
    }
}
