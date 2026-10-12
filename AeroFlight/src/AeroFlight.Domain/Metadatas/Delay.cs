using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public abstract class Delay : FlightResourceBase
{
    public string DelayCode { get; private set; } = string.Empty;
    public decimal? DelayMinutes { get; private set; }
    public string? Comment { get; private set; }

    protected Delay() { }

    protected Delay(long flightId, string delayCode, decimal? delayMinutes = null, string? comment = null)
        : base(flightId)
    {
        DelayCode = delayCode;
        DelayMinutes = delayMinutes;
        Comment = comment;
    }

    public void Update(string delayCode, decimal? delayMinutes = null, string? comment = null)
    {
        DelayCode = delayCode;
        DelayMinutes = delayMinutes;
        Comment = comment;
    }
}
