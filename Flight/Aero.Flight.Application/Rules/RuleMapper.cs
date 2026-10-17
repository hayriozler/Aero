using Aero.Flights.Contracts.Rules;
using Aero.Flights.Domain.Rules;

namespace Aero.Flights.Application.Rules;

internal static class RuleMapper
{
    public static RuleResponse ToResponse(Rule rule) => new(
        rule.Id,
        rule.Name,
        rule.Description,
        [.. rule.Triggers.Select(t => t.Trigger.ToString())],
        rule.IsEnabled,
        rule.StopOnMatch,
        [.. rule.ConditionGroups.OrderBy(g => g.Order).Select(g => new RuleConditionGroupResponse(
            g.Id,
            g.Operator.ToString(),
            [.. g.Conditions.OrderBy(c => c.Id).Select(c => new RuleConditionResponse(c.Id, c.Field, c.Operator.ToString(), c.Value))]))],
        [.. rule.Actions
            .OrderBy(a => a.Order)
            .Select(a => new RuleActionResponse(a.Id, a.ActionType.ToString(), a.TargetField, a.Value, a.Order, a.MatchValue))],
        rule.CreatedAt,
        rule.CreatedBy,
        rule.UpdatedAt,
        rule.UpdatedBy);

    public static void ApplyRequest(Rule rule, RuleRequest request)
    {
        rule.Update(
            request.Name,
            request.Description,
            ParseTriggers(request.Triggers),
            request.IsEnabled,
            request.StopOnMatch);

        rule.ClearConditionGroups();
        AddConditionGroups(rule, request.ConditionGroups);

        rule.ClearActions();
        foreach (var a in request.Actions)
            rule.AddAction(Enum.Parse<RuleActionType>(a.ActionType, ignoreCase: true), a.TargetField, a.Value, a.MatchValue, a.Order);
    }

    public static Rule FromRequest(RuleRequest request)
    {
        var rule = Rule.Create(
            request.Name,
            request.Description,
            ParseTriggers(request.Triggers),
            request.IsEnabled,
            request.StopOnMatch);

        AddConditionGroups(rule, request.ConditionGroups);

        foreach (var a in request.Actions)
            rule.AddAction(Enum.Parse<RuleActionType>(a.ActionType, ignoreCase: true), a.TargetField, a.Value, a.MatchValue, a.Order);

        return rule;
    }

    private static void AddConditionGroups(Rule rule, IReadOnlyList<RuleConditionGroupRequest> groups)
    {
        foreach (var g in groups)
        {
            var group = rule.AddConditionGroup(Enum.Parse<ConditionLogicalOperator>(g.Operator, ignoreCase: true));
            foreach (var c in g.Conditions)
                group.AddCondition(c.Field, Enum.Parse<ConditionOperator>(c.Operator, ignoreCase: true), c.Value);
        }
    }

    private static IEnumerable<RuleTrigger> ParseTriggers(IReadOnlyList<string> triggers) =>
        triggers.Select(t => Enum.Parse<RuleTrigger>(t, ignoreCase: true));
}
