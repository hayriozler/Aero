using AeroFlight.Domain.Rules;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Migrations.Seeders;

// DEBUG-only test fixture — ground-resource requirement rules for two aircraft-size classes, used
// to exercise the Business Rules Engine and to give FlightSeeder's generated flights something to
// build resource assignments against. RequiredGates/RequiredBelts auto-adapt to whichever direction
// the flight actually is, but RequiredCounters is departure-only (see FlightRuleFieldCatalog) — so
// each aircraft class needs one arrival-triggered and one departure-triggered rule, not a single
// direction-agnostic one.
internal sealed class TestRulesSeeder(AeroFlightDbContext db, ILogger<TestRulesSeeder> logger)
{
    private const string _namePrefix = "Test: ";

    public async Task SeedAsync(CancellationToken ct)
    {
        var rules = new[]
        {
            BuildRule($"{_namePrefix}Narrow-body departure resources", RuleTrigger.DepartureCreated,
                counters: 2, gates: 1, belts: 1),
            BuildRule($"{_namePrefix}Narrow-body arrival resources", RuleTrigger.ArrivalCreated,
                counters: null, gates: 1, belts: 1),
            BuildRule($"{_namePrefix}Wide-body departure resources", RuleTrigger.DepartureCreated,
                counters: 4, gates: 2, belts: 1, wideBody: true),
            BuildRule($"{_namePrefix}Wide-body arrival resources", RuleTrigger.ArrivalCreated,
                counters: null, gates: 2, belts: 2, wideBody: true),
            BuildNoPassengerCompletionGuard(),
        };

        // Per-rule (not "any test rule exists") so a rule added to this seeder later still gets
        // created in a tenant DB that was already seeded with an earlier version of this list.
        var existingNames = await db.RuleSet
            .Where(r => r.Name.StartsWith(_namePrefix))
            .Select(r => r.Name)
            .ToHashSetAsync(ct);

        var missing = rules.Where(r => !existingNames.Contains(r.Name)).ToList();
        if (missing.Count == 0)
        {
            logger.LogInformation("Test business rules already seeded.");
            return;
        }

        db.RuleSet.AddRange(missing);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} test business rule(s).", missing.Count);
    }

    private static Rule BuildRule(string name, RuleTrigger trigger, int? counters, int gates, int belts, bool wideBody = false)
    {
        var rule = Rule.Create(name,
            wideBody ? "Aircraft type starts with B74 or A38" : "Aircraft type starts with B73 or A32",
            [trigger], isEnabled: true, stopOnMatch: false);

        var group = rule.AddConditionGroup(ConditionLogicalOperator.Or);
        if (wideBody)
        {
            group.AddCondition("AircraftTypeCode", ConditionOperator.StartsWith, "B74");
            group.AddCondition("AircraftTypeCode", ConditionOperator.StartsWith, "A38");
        }
        else
        {
            group.AddCondition("AircraftTypeCode", ConditionOperator.StartsWith, "B73");
            group.AddCondition("AircraftTypeCode", ConditionOperator.StartsWith, "A32");
        }

        var order = 0;
        if (counters is { } c)
            rule.AddAction(RuleActionType.RequireResourceCount, "RequiredCounters", c.ToString(), null, order++);
        rule.AddAction(RuleActionType.RequireResourceCount, "RequiredGates", gates.ToString(), null, order++);
        rule.AddAction(RuleActionType.RequireResourceCount, "RequiredBelts", belts.ToString(), null, order);

        return rule;
    }

    // "Complete" a departure = status set to COM (Completed Flight) — confirmed against actual
    // operator usage (OFB was tried and ruled out). J = Scheduled Passenger (see flight_types.csv)
    // — a scheduled passenger flight shouldn't be marked completed with no passengers ever recorded
    // (PaxTotal is nullable, so both null and 0 count as "no passenger").
    private static Rule BuildNoPassengerCompletionGuard()
    {
        var rule = Rule.Create(
            $"{_namePrefix}Block empty-passenger scheduled flight completion",
            "Scheduled Passenger (J) departures can't be marked completed (COM) with no passengers recorded",
            [RuleTrigger.DepartureUpdated], isEnabled: true, stopOnMatch: false);

        var typeAndStatus = rule.AddConditionGroup(ConditionLogicalOperator.And);
        typeAndStatus.AddCondition("FlightTypeCode", ConditionOperator.Equals, "J");
        typeAndStatus.AddCondition("Status", ConditionOperator.Equals, "COM");

        var noPax = rule.AddConditionGroup(ConditionLogicalOperator.Or);
        noPax.AddCondition("PaxTotal", ConditionOperator.IsNull, null);
        noPax.AddCondition("PaxTotal", ConditionOperator.Equals, "0");

        rule.AddAction(RuleActionType.RaiseError, "ValidationError",
            "Cannot complete a Scheduled Passenger flight with no passengers recorded.", null, 0);

        return rule;
    }
}
