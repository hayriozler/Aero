using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Scheduling;

public sealed class JobSchedule : AuditBase<string>
{
    public string JobName { get; private set; } = string.Empty;
    public string? CronExpression { get; private set; }
    public int? IntervalSeconds { get; private set; }
    public bool IsEnabled { get; private set; } = true;

    private JobSchedule() { }

    private JobSchedule(string jobName, string? cronExpression, int? intervalSeconds)
    {
        SetId(jobName);
        JobName = jobName;
        CronExpression = cronExpression;
        IntervalSeconds = intervalSeconds;
    }

    public static JobSchedule Create(string jobName, string? cronExpression = null, int? intervalSeconds = null)
        => new(jobName, cronExpression, intervalSeconds);

    public void UpdateSchedule(string? cronExpression, int? intervalSeconds)
    {
        CronExpression = cronExpression;
        IntervalSeconds = intervalSeconds;
    }

    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
}
