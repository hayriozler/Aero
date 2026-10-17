using Aero.Flights.Domain.Common;

namespace Aero.Flights.Domain.Ops.Resources;

public abstract class ResourceBase : FlightResourceBase
{
    public string? Code { get; private set; }
    public string? LastCode { get; private set; }
    public DateTimeOffset? BeginPlan { get; private set; }
    public DateTimeOffset? EndPlan { get; private set; }
    public DateTimeOffset? BeginActual { get; private set; }
    public DateTimeOffset? EndActual { get; private set; }
    public string? DisplayCode { get; private set; }
    public string? StatusCode { get; private set; }
    public string? PayerCompanyCode { get; private set; }
    public bool IsComplimentary { get; private set; }

    protected ResourceBase() { }

    protected ResourceBase(long flightId, string? code) : base(flightId) => Code = code;

    public void SetPlan(DateTimeOffset? beginPlan, DateTimeOffset? endPlan)
    {
        BeginPlan = beginPlan;
        EndPlan = endPlan;
    }

    public void RecordActual(DateTimeOffset? beginActual, DateTimeOffset? endActual = null)
    {
        BeginActual = beginActual;
        EndActual = endActual;
    }

    public void UpdateStatus(string? statusCode) => StatusCode = statusCode;

    public void UpdateCode(string? code, string? lastCode = null)
    {
        LastCode = lastCode ?? Code;
        Code = code;
    }

    public void UpdateDisplayCode(string? displayCode) => DisplayCode = displayCode;

    public void UpdatePayer(string? payerCompanyCode) => PayerCompanyCode = payerCompanyCode;

    public void UpdateComplimentary(bool isComplimentary) => IsComplimentary = isComplimentary;
}
