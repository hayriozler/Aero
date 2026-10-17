using Aero.Flights.Domain.Common;

namespace Aero.Flights.Domain.Ops.Services;

public abstract class OpsService : FlightResourceBase
{
    public string ServiceCode { get; private set; } = string.Empty;
    public decimal? AmountPlan { get; private set; }
    public decimal? AmountActual { get; private set; }
    public DateTimeOffset? BeginPlan { get; private set; }
    public DateTimeOffset? EndPlan { get; private set; }
    public DateTimeOffset? BeginActual { get; private set; }
    public DateTimeOffset? EndActual { get; private set; }
    public string? HandlingAgentCode { get; private set; }
    public string? PayerCompanyCode { get; private set; }
    public string? Comment { get; private set; }

    protected OpsService() { }

    protected OpsService(long flightId, string serviceCode) : base(flightId) => ServiceCode = serviceCode;

    public void ChangeServiceCode(string serviceCode) => ServiceCode = serviceCode;

    public void SetPlan(DateTimeOffset? beginPlan, DateTimeOffset? endPlan, decimal? amountPlan = null)
    {
        BeginPlan = beginPlan;
        EndPlan = endPlan;
        AmountPlan = amountPlan;
    }

    public void RecordActual(DateTimeOffset? beginActual, DateTimeOffset? endActual, decimal? amountActual = null)
    {
        BeginActual = beginActual;
        EndActual = endActual;
        AmountActual = amountActual;
    }

    public void UpdateHandlingAgent(string? handlingAgentCode) => HandlingAgentCode = handlingAgentCode;
    public void UpdatePayer(string? payerCompanyCode) => PayerCompanyCode = payerCompanyCode;
    public void UpdateComment(string? comment) => Comment = comment;
}
