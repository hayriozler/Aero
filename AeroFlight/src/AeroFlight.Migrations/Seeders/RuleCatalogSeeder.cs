using AeroFlight.Application.Rules;
using AeroFlight.Domain.Rules;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Migrations.Seeders;

// Resyncs Metadata.RuleActionType/RuleTriggerOption/RuleField/RuleFieldTrigger/RuleFieldActionType from
// FlightRuleFieldCatalog on every run (delete + reinsert, not insert-only like AeroConfigSeeder) —
// these rows are a generated mirror of the code catalog for Metadata-schema browsing/filtering,
// never hand-edited, so resyncing can't lose anything and keeps them from ever drifting out of
// sync with what the rule engine actually executes.
internal sealed partial class RuleCatalogSeeder(AeroFlightDbContext db, FlightRuleFieldCatalog catalog, ILogger<RuleCatalogSeeder> logger)
{
    private static readonly (string Code, string Description)[] _actionTypes =
    [
        (nameof(RuleActionType.SetField), "Set Field"),
        (nameof(RuleActionType.AppendField), "Append To Field"),
        (nameof(RuleActionType.RequireResourceCount), "Require Resource Count"),
        (nameof(RuleActionType.Finance), "Finance (Assign Payer)"),
        (nameof(RuleActionType.RaiseError), "Raise Error (Block Save)"),
    ];

    private static readonly (string Code, string Description)[] _triggers =
    [
        (nameof(RuleTrigger.FlightCreated), "Flight Created (Any Direction)"),
        (nameof(RuleTrigger.FlightUpdated), "Flight Updated (Any Direction)"),
        (nameof(RuleTrigger.FlightDeleted), "Flight Deleted (Any Direction)"),
        (nameof(RuleTrigger.DepartureCreated), "Departure Created"),
        (nameof(RuleTrigger.DepartureUpdated), "Departure Update"),
        (nameof(RuleTrigger.DepartureDeleted), "Departure Delete"),
        (nameof(RuleTrigger.CounterCreated), "Counter Created"),
        (nameof(RuleTrigger.CounterUpdated), "Counter Updated"),
        (nameof(RuleTrigger.CounterDeleted), "Counter Deleted"),
        (nameof(RuleTrigger.GateCreated), "Gate Created"),
        (nameof(RuleTrigger.GateUpdated), "Gate Updated"),
        (nameof(RuleTrigger.GateDeleted), "Gate Deleted"),
        (nameof(RuleTrigger.ParkPositionCreated), "ParkPosition Created"),
        (nameof(RuleTrigger.ParkPositionUpdated), "ParkPosition Updated"),
        (nameof(RuleTrigger.ParkPositionDeleted), "ParkPosition Deleted"),
        (nameof(RuleTrigger.DepartureBeltCreated), "Departure ArrivalBelt Created"),
        (nameof(RuleTrigger.DepartureBeltUpdated), "Departure ArrivalBelt Updated"),
        (nameof(RuleTrigger.DepartureBeltDeleted), "Departure ArrivalBelt Deleted"),
        (nameof(RuleTrigger.ArrivalCreated), "Arrival Created"),
        (nameof(RuleTrigger.ArrivalUpdated), "Arrival Updated"),
        (nameof(RuleTrigger.ArrivalDeleted), "Arrival Deleted"),
        (nameof(RuleTrigger.ArrivalBeltCreated), "Arrival ArrivalBelt Created"),
        (nameof(RuleTrigger.ArrivalBeltUpdated), "Arrival ArrivalBelt Updated"),
        (nameof(RuleTrigger.ArrivalBeltDeleted), "Arrival ArrivalBelt Deleted"),
    ];

    public async Task SeedAsync(CancellationToken ct)
    {
        await db.RuleFieldActionTypeMetadataSet.ExecuteDeleteAsync(ct);
        await db.RuleFieldTriggerMetadataSet.ExecuteDeleteAsync(ct);
        await db.RuleFieldMetadataSet.ExecuteDeleteAsync(ct);
        await db.RuleActionTypeMetadataSet.ExecuteDeleteAsync(ct);
        await db.RuleTriggerMetadataSet.ExecuteDeleteAsync(ct);

        foreach (var (code, description) in _actionTypes)
            db.Database.ExecuteSqlRaw(
                "INSERT INTO Metadata.\"RuleActionType\" (\"Code\", \"Description\") VALUES ({0}, {1})", code, description);

        foreach (var (code, description) in _triggers)
            db.Database.ExecuteSqlRaw(
                "INSERT INTO Metadata.\"RuleTriggerOption\" (\"Code\", \"Description\") VALUES ({0}, {1})", code, description);

        foreach (var field in catalog.Fields)
        {
            db.Database.ExecuteSqlRaw(
                "INSERT INTO Metadata.\"RuleField\" (\"Key\", \"Label\", \"DataType\", \"UsableInCondition\", \"UsableInAction\", \"SupportsMatchValue\", \"MatchValueLabel\") VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                field.Key, field.Label, field.DataType.ToString(), field.UsableInCondition, field.UsableInAction, field.SupportsMatchValue, field.MatchValueLabel);

            foreach (var trigger in field.Triggers)
                db.Database.ExecuteSqlRaw(
                    "INSERT INTO Metadata.\"RuleFieldTrigger\" (\"RuleFieldKey\", \"Trigger\") VALUES ({0}, {1})",
                    field.Key, trigger.ToString());

            foreach (var actionType in field.SupportedActionTypes ?? [])
                db.Database.ExecuteSqlRaw(
                    "INSERT INTO Metadata.\"RuleFieldActionType\" (\"RuleFieldKey\", \"ActionTypeCode\") VALUES ({0}, {1})",
                    field.Key, actionType.ToString());
        }

        LogSynced(logger, catalog.Fields.Count);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Rule catalog Metadata tables synced ({Count} fields).")]
    private static partial void LogSynced(ILogger logger, int count);
}
