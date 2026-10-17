namespace Aero.Flights.Domain.Identity;

public static class Permissions
{
    public const string MetadataView = "Metadata.view";
    public const string MetadataCreate = "Metadata.create";
    public const string MetadataUpdate = "Metadata.update";
    public const string MetadataDelete = "Metadata.delete";
    public const string FlightView = "flight.view";
    public const string FlightWrite = "flight.write";
    public const string SeasonalFlightScheduleView = "seasonalflightschedule.view";
    public const string SeasonalFlightScheduleWrite = "seasonalflightschedule.write";
    public const string UserManage = "user.manage";
    public const string RoleManage = "role.manage";
    public const string RuleManage = "rule.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (MetadataView, "View Metadata data"),
        (MetadataCreate, "Create Metadata data"),
        (MetadataUpdate, "Update Metadata data"),
        (MetadataDelete, "Delete Metadata data"),
        (FlightView, "View flights"),
        (FlightWrite, "Create and update flights"),
        (SeasonalFlightScheduleView, "View seasonal flight schedules"),
        (SeasonalFlightScheduleWrite, "Create and update seasonal flight schedules"),
        (UserManage, "Manage users"),
        (RoleManage, "Manage roles and permissions"),
        (RuleManage, "Manage rules"),
    ];
}
