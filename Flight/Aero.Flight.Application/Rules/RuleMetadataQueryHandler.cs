using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Rules;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Rules;

public sealed record GetRuleMetadataQuery;

public sealed class RuleMetadataQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetRuleMetadataQuery, RuleMetadataResponse>
{
    private static readonly IReadOnlyList<RuleOperatorOptionResponse> _operators =
    [
        new(nameof(ConditionOperator.Equals), "Equals", "single"),
        new(nameof(ConditionOperator.NotEquals), "Not Equals", "single"),
        new(nameof(ConditionOperator.In), "In List", "list"),
        new(nameof(ConditionOperator.GreaterThan), "Greater Than", "single"),
        new(nameof(ConditionOperator.LessThan), "Less Than", "single"),
        new(nameof(ConditionOperator.StartsWith), "Starts With", "single"),
        new(nameof(ConditionOperator.Contains), "Contains", "single"),
        new(nameof(ConditionOperator.IsNull), "Is Empty", "none"),
        new(nameof(ConditionOperator.IsNotNull), "Is Not Empty", "none"),
    ];

    // Triggers, Action Types and Fields (with their Trigger/ActionType associations) come from the
    // Metadata.RuleTriggerOption/RuleActionType/RuleField/RuleFieldTrigger/RuleFieldActionType tables —
    // kept in sync with FlightRuleFieldCatalog/RuleTrigger/RuleActionType by RuleCatalogSeeder on
    // every tenant seed run, so this is a real Metadata-schema lookup rather than an in-memory
    // array, while the actual Getter/Setter that executes a rule action still only ever comes from
    // the code catalog.
    public async Task<RuleMetadataResponse> HandleAsync(GetRuleMetadataQuery query, CancellationToken ct)
    {
        var triggers = await db.Query<RuleTriggerMetadata>()
            .Select(t => new RuleOptionResponse(t.Code, t.Description ?? t.Code))
            .ToListAsync(ct);

        var actionTypes = await db.Query<RuleActionTypeMetadata>()
            .Select(a => new RuleOptionResponse(a.Code, a.Description ?? a.Code))
            .ToListAsync(ct);

        var fieldRows = await db.Query<RuleFieldMetadata>().ToListAsync(ct);
        var triggerRows = await db.Query<RuleFieldTriggerMetadata>().ToListAsync(ct);
        var actionTypeRows = await db.Query<RuleFieldActionTypeMetadata>().ToListAsync(ct);

        var triggersByField = triggerRows
            .GroupBy(t => t.RuleFieldKey)
            .ToDictionary(g => g.Key, g => g.Select(t => t.Trigger).ToList());

        var actionTypesByField = actionTypeRows
            .GroupBy(a => a.RuleFieldKey)
            .ToDictionary(g => g.Key, g => g.Select(a => a.ActionTypeCode).ToList());

        var fields = fieldRows
            .Select(f => new RuleFieldOptionResponse(
                f.Key,
                f.Label,
                f.DataType,
                triggersByField.GetValueOrDefault(f.Key, []),
                f.UsableInCondition,
                f.UsableInAction,
                null,
                actionTypesByField.GetValueOrDefault(f.Key),
                f.SupportsMatchValue,
                f.MatchValueLabel))
            .ToList();

        return new RuleMetadataResponse(triggers, _operators, actionTypes, fields);
    }
}
