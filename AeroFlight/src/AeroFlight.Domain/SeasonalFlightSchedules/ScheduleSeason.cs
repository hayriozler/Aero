using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.SeasonalFlightSchedules;

public sealed class ScheduleSeason : AuditBase<string>
{
    public string? Description { get; private set; }
    public DateTimeOffset BeginDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }

    private ScheduleSeason() { }

    private ScheduleSeason(string code, DateTimeOffset beginDate, DateTimeOffset endDate, string? description)
    {
        SetId(code);
        BeginDate = beginDate;
        EndDate = endDate;
        Description = description;
    }

    public static ScheduleSeason Create(string code, DateTimeOffset beginDate, DateTimeOffset endDate, string? description = null)
        => new(code, beginDate, endDate, description);

    public void UpdateDates(DateTimeOffset beginDate, DateTimeOffset endDate)
    {
        BeginDate = beginDate;
        EndDate = endDate;
    }

    public void UpdateDescription(string description) => Description = description;
}
