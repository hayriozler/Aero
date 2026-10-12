namespace AeroFids.Infrastructure.Flights;

public static class InternalFlightStatuses
{
    public const string Scheduled = "SCHEDULED";
    public const string Boarding = "BOARDING";
    public const string Departed = "DEPARTED";
    public const string Landed = "LANDED";
    public const string Cancelled = "CANCELLED";
    public const string Diverted = "DIVERTED";
    public const string Unknown = "UNKNOWN";

    public static readonly IReadOnlyList<string> All = [Scheduled, Boarding, Departed, Landed, Cancelled, Diverted, Unknown];
}

public static class InternalResourceStatuses
{
    public const string Open = "OPEN";
    public const string Closed = "CLOSED";
    public const string Unknown = "UNKNOWN";

    public static readonly IReadOnlyList<string> All = [Open, Closed, Unknown];
}
