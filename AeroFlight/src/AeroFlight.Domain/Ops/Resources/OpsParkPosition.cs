using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Ops.Resources;

public sealed class OpsFlightParkPosition: AuditBase<long>
{
    public long? ArrivalFlightId { get; private set; }
    public long? DepartureFlightId { get; private set; }
    public string? Code { get; private set; }
    public string? LastCode { get; private set; }
    public int Order { get; private set; }
    public DateTimeOffset BeginPlan { get; private set; }
    public DateTimeOffset EndPlan { get; private set; }
    public DateTimeOffset? BeginActual { get; private set; }
    public DateTimeOffset? EndActual { get; private set; }
    public string? TowCode { get; private set; }
    public string? TowIndicator { get; private set; }
    public string? HandlingAgentCode { get; private set; }
    public string? PayerCompanyCode { get; private set; }
    public string? FlightStatusCode { get; private set; }
    public ArrivalFlight? ArrivalFlight { get; private set; }
    public DepartureFlight? DepartureFlight { get; private set; }

    private OpsFlightParkPosition() { }

    private OpsFlightParkPosition(long? arrivalFlightId, long? departureFlightId, DateTimeOffset beginPlan, DateTimeOffset endPlan, int order)
    {
        ArrivalFlightId = arrivalFlightId;
        DepartureFlightId = departureFlightId;
        BeginPlan = beginPlan;
        EndPlan = endPlan;
        Order = order;
    }

    public static OpsFlightParkPosition CreateForArrival(long arrivalFlightId, DateTimeOffset beginPlan, DateTimeOffset endPlan, int order = 1)
        => new(arrivalFlightId, null, beginPlan, endPlan, order);

    public static OpsFlightParkPosition CreateForDeparture(long departureFlightId, DateTimeOffset beginPlan, DateTimeOffset endPlan, int order = 1)
        => new(null, departureFlightId, beginPlan, endPlan, order);

    public static OpsFlightParkPosition CreateForTurnaround(long arrivalFlightId, long departureFlightId, DateTimeOffset beginPlan, DateTimeOffset endPlan, int order = 1)
        => new(arrivalFlightId, departureFlightId, beginPlan, endPlan, order);

    public void AssignParkPosition(string? parkpositionCode, string? lastParkPositionCode = null)
    {
        LastCode = lastParkPositionCode ?? Code;
        Code = parkpositionCode;
    }

    public void UpdatePlan(DateTimeOffset beginPlan, DateTimeOffset endPlan)
    {
        BeginPlan = beginPlan;
        EndPlan = endPlan;
    }

    public void RecordActual(DateTimeOffset beginActual, DateTimeOffset? endActual = null)
    {
        BeginActual = beginActual;
        EndActual = endActual;
    }

    public void UpdateTow(string? towCode, string? towIndicator = null)
    {
        TowCode = towCode;
        TowIndicator = towIndicator;
    }

    public void UpdatePayer(string? payerCompanyCode) => PayerCompanyCode = payerCompanyCode;
}
