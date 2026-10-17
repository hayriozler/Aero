using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Domain.Ops;

namespace Aero.Flights.Application.Flights.InboundProcessors;

internal static class FlightMessageMapper
{
    public static void ApplyUpdate(this ArrivalFlight f, ArrivalFlightUpdated m)
    {
        Apply(m.StatusCode, v => f.FlightUpdateStatus(v));
        Apply(m.Eta, v => f.UpdateEta(v));
        Apply(m.Ata, v => f.RecordAta(v));
        Apply(m.OnBlock, v => f.RecordOnBlock(v));
        Apply(m.TerminalCode, v => f.UpdateTerminal(v));
        Apply(m.ParkPositionCode, v => f.UpdateParkPosition(v));
        Apply(m.PaxTotal, v => f.UpdatePaxCounts(v, null, null, null, null, null, null, null));
    }

    public static void ApplyUpdate(this DepartureFlight f, DepartureFlightUpdated m)
    {
        Apply(m.FlightStatusCode, v => f.FlightUpdateStatus(v));
        Apply(m.Etd, v => f.UpdateEtd(v));
        Apply(m.Atd, v => f.RecordAtd(v));
        Apply(m.OffBlock, v => f.RecordOffBlock(v));
        Apply(m.TerminalCode, v => f.UpdateTerminal(v));
        Apply(m.ParkPositionCode, v => f.UpdateParkPosition(v));
        Apply(m.PaxTotal, v => f.UpdatePaxCounts(v, null, null, null, null, null, null, null));
    }

    private static void Apply<T>(T? value, Action<T> apply) where T : struct
    {
        if (value is { } v) apply(v);
    }

    private static void Apply(string? value, Action<string> apply)
    {
        if (value is not null) apply(value);
    }
}
