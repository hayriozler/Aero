using Aero.Flights.Application.Interfaces;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Delays;
using Aero.Flights.Domain.Ops.Passengers;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Ops.Routing;
using Aero.Flights.Domain.Ops.Services;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Metadatas;

// Covers every FK that points at Airline (checked against HasOne<Airline>() across all
// configurations, not just the ones explicitly set to Restrict) — AirlineContact/AirlineLogo are
// excluded on purpose since those cascade (they're Airline's own child rows, not a blocker).
public sealed class AirlineUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Airline>
{
    public async Task<string?> GetBlockReasonAsync(Airline entity, CancellationToken ct)
    {
        var code = entity.Id;

        // AnyAsync short-circuits both in .NET (|| stops at the first true) and in Postgres
        // (EXISTS stops scanning at the first match) — no row count needed, just in-use or not.
        var inUse =
            await db.Query<ArrivalFlight>().AnyAsync(f => f.AirlineCode == code || f.OperatingAirlineCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.AirlineCode == code || f.OperatingAirlineCode == code, ct)
            || await db.Query<OpsArrivalCodeShare>().AnyAsync(c => c.AirlineCode == code, ct)
            || await db.Query<OpsDepartureCodeShare>().AnyAsync(c => c.AirlineCode == code, ct)
            || await db.Query<Aircraft>().AnyAsync(a => a.AirlineCode == code, ct)
            || await db.Query<Airline>().AnyAsync(a => a.ParentAirlineCode == code, ct);

        return inUse ? $"Airline '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class FlightTypeUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<FlightType>
{
    public async Task<string?> GetBlockReasonAsync(FlightType entity, CancellationToken ct)
    {
        var code = entity.Code;

        var inUse =
            await db.Query<ArrivalFlight>().AnyAsync(f => f.FlightTypeCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.FlightTypeCode == code, ct);

        return inUse ? $"Flight type '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class AircraftTypeUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<AircraftType>
{
    public async Task<string?> GetBlockReasonAsync(AircraftType entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<Aircraft>().AnyAsync(a => a.AircraftTypeCode == code, ct)
            || await db.Query<ArrivalFlight>().AnyAsync(f => f.AircraftTypeCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.AircraftTypeCode == code, ct);

        return inUse ? $"Aircraft type '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class AirportUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Airport>
{
    public async Task<string?> GetBlockReasonAsync(Airport entity, CancellationToken ct)
    {
        var code = entity.Id;

        // OriginAirportCode/DestinationAirportCode aren't stored columns — they're computed from
        // the leg-0 OpsRouting row (see ArrivalFlight.OriginAirportCode/DepartureFlight.
        // DestinationAirportCode), and OpsRouting.Code has no FK to Airport either (same loose
        // string-code situation as Gate/Counter/HandlingService), so every routing leg needs checking
        // here, not just DivertAirportCode/DepDestination which do have real FKs.
        var inUse =
            await db.Query<ArrivalFlight>().AnyAsync(f => f.DivertAirportCode == code, ct)
            || await db.Query<OpsTransferPassenger>().AnyAsync(t => t.DepDestination == code, ct)
            || await db.Query<OpsArrivalRouting>().AnyAsync(r => r.Code == code, ct)
            || await db.Query<OpsDepartureRouting>().AnyAsync(r => r.Code == code, ct);

        return inUse ? $"Airport '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class AircraftUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Aircraft>
{
    public async Task<string?> GetBlockReasonAsync(Aircraft entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<ArrivalFlight>().AnyAsync(f => f.AircraftCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.AircraftCode == code, ct);

        return inUse ? $"Aircraft '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class AircraftSizeCategoryUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<AircraftSizeCategory>
{
    public async Task<string?> GetBlockReasonAsync(AircraftSizeCategory entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<Aircraft>().AnyAsync(a => a.SizeCategory == code, ct)
            || await db.Query<AircraftType>().AnyAsync(a => a.SizeCategory == code, ct);

        return inUse ? $"Aircraft size category '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class ParkPositionUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<ParkPosition>
{
    public async Task<string?> GetBlockReasonAsync(ParkPosition entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<ArrivalFlight>().AnyAsync(f => f.ParkPositionCode == code || f.CurrentParkPositionCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.ParkPositionCode == code || f.CurrentParkPositionCode == code, ct);

        return inUse ? $"Park position '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class TerminalUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Terminal>
{
    public async Task<string?> GetBlockReasonAsync(Terminal entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<ArrivalFlight>().AnyAsync(f => f.TerminalCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.TerminalCode == code, ct);

        return inUse ? $"Terminal '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class CountryUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Country>
{
    public async Task<string?> GetBlockReasonAsync(Country entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<Airline>().AnyAsync(a => a.CountryCode == code, ct)
            || await db.Query<Airport>().AnyAsync(a => a.CountryCode == code, ct);

        return inUse ? $"Country '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class CompanyTypeUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<CompanyType>
{
    public async Task<string?> GetBlockReasonAsync(CompanyType entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse = await db.Query<Company>().AnyAsync(c => c.CompanyTypeCode == code, ct);

        return inUse ? $"Company type '{code}' is in use and cannot be deleted." : null;
    }
}

// Gate/Counter/HandlingService have no DB-level FK from the ops resource tables — OpsGate/OpsCounter/
// OpsService store the assigned code as a plain string, not a real foreign key — so without this
// guard, deleting one of these while a flight still Metadatas it by code would silently orphan
// that flight's resource/service row instead of being blocked. This is a rule check, not a
// DB backstop, since there's no Restrict FK underneath it.
public sealed class GateUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Gate>
{
    public async Task<string?> GetBlockReasonAsync(Gate entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<OpsArrivalGate>().AnyAsync(g => g.Code == code, ct)
            || await db.Query<OpsDepartureGate>().AnyAsync(g => g.Code == code, ct);

        return inUse ? $"Gate '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class CounterUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Counter>
{
    public async Task<string?> GetBlockReasonAsync(Counter entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse = await db.Query<OpsCounter>().AnyAsync(c => c.Code == code, ct);

        return inUse ? $"Counter '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class HandlingServiceUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<HandlingService>
{
    public async Task<string?> GetBlockReasonAsync(HandlingService entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<OpsArrivalService>().AnyAsync(s => s.ServiceCode == code, ct)
            || await db.Query<OpsDepartureService>().AnyAsync(s => s.ServiceCode == code, ct);

        return inUse ? $"Handling service '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class ArrivalBeltUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<ArrivalBelt>
{
    public async Task<string?> GetBlockReasonAsync(ArrivalBelt entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse = await db.Query<OpsArrivalBelt>().AnyAsync(b => b.Code == code, ct);

        return inUse ? $"Arrival belt '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class DepartureBeltUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<DepartureBelt>
{
    public async Task<string?> GetBlockReasonAsync(DepartureBelt entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse = await db.Query<OpsDepartureBelt>().AnyAsync(b => b.Code == code, ct);

        return inUse ? $"Departure belt '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class CheckinClassUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<CheckinClass>
{
    public async Task<string?> GetBlockReasonAsync(CheckinClass entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse = await db.Query<OpsCounter>().AnyAsync(c => c.CheckinClassCode == code, ct);

        return inUse ? $"Check-in class '{code}' is in use and cannot be deleted." : null;
    }
}

public sealed class DelayReasonUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<DelayReason>
{
    public async Task<string?> GetBlockReasonAsync(DelayReason entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<OpsArrivalDelay>().AnyAsync(d => d.DelayCode == code, ct)
            || await db.Query<OpsDepartureDelay>().AnyAsync(d => d.DelayCode == code, ct);

        return inUse ? $"Delay reason '{code}' is in use and cannot be deleted." : null;
    }
}

// Company is Metadatad two ways, neither a real FK: as a payer (PayerCompanyCode, on every
// ResourceBase-derived resource plus Service/ParkPosition) and as a handling agent
// (HandlingAgentCode, on Flight/Service/ParkPosition). Covers both across every resource kind.
public sealed class CompanyUsageGuard(IAeroFlightDbContext db) : IMetadataUsageGuard<Company>
{
    public async Task<string?> GetBlockReasonAsync(Company entity, CancellationToken ct)
    {
        var code = entity.Id;

        var inUse =
            await db.Query<OpsArrivalGate>().AnyAsync(r => r.PayerCompanyCode == code, ct)
            || await db.Query<OpsDepartureGate>().AnyAsync(r => r.PayerCompanyCode == code, ct)
            || await db.Query<OpsArrivalBelt>().AnyAsync(r => r.PayerCompanyCode == code, ct)
            || await db.Query<OpsDepartureBelt>().AnyAsync(r => r.PayerCompanyCode == code, ct)
            || await db.Query<OpsCounter>().AnyAsync(r => r.PayerCompanyCode == code, ct)
            || await db.Query<OpsArrivalService>().AnyAsync(s => s.PayerCompanyCode == code || s.HandlingAgentCode == code, ct)
            || await db.Query<OpsDepartureService>().AnyAsync(s => s.PayerCompanyCode == code || s.HandlingAgentCode == code, ct)
            || await db.Query<OpsFlightParkPosition>().AnyAsync(p => p.PayerCompanyCode == code || p.HandlingAgentCode == code, ct)
            || await db.Query<ArrivalFlight>().AnyAsync(f => f.HandlingAgentCode == code, ct)
            || await db.Query<DepartureFlight>().AnyAsync(f => f.HandlingAgentCode == code, ct);

        return inUse ? $"Company '{code}' is in use and cannot be deleted." : null;
    }
}
