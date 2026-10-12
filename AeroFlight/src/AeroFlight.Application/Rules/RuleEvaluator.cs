using System.Globalization;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Rules;

namespace AeroFlight.Application.Rules;

// Pure, side-effect-free rule matching/mutation over a flight — no I/O, unit-testable in isolation.
public static class RuleEvaluator
{
    public static void Apply(IEnumerable<Rule> rules, Flight flight, FlightRuleFieldCatalog catalog, ResourceDefaults resourceDefaults)
    {
        foreach (var rule in rules.OrderBy(r => r.Id))
        {
            if (!Matches(rule, flight, catalog)) continue;

            foreach (var action in rule.Actions.OrderBy(a => a.Order))
                ApplyAction(action, flight, catalog, resourceDefaults);

            if (rule.StopOnMatch) break;
        }
    }

    // Groups are always ANDed together at the rule level; each group's own conditions combine
    // via its own single Operator (all AND, or all OR) — see RuleConditionGroup.
    private static bool Matches(Rule rule, Flight flight, FlightRuleFieldCatalog catalog)
    {
        if (rule.ConditionGroups.Count == 0)
            return true;

        return rule.ConditionGroups.OrderBy(g => g.Order).All(g => MatchesGroup(g, flight, catalog));
    }

    private static bool MatchesGroup(RuleConditionGroup group, Flight flight, FlightRuleFieldCatalog catalog)
    {
        if (group.Conditions.Count == 0)
            return true;

        return group.Operator == ConditionLogicalOperator.Or
            ? group.Conditions.Any(c => Evaluate(c, flight, catalog))
            : group.Conditions.All(c => Evaluate(c, flight, catalog));
    }

    private static bool Evaluate(RuleCondition condition, Flight flight, FlightRuleFieldCatalog catalog)
    {
        var field = catalog.Find(condition.Field);
        if (field is null) return false;

        var actual = ToStringValue(field.Getter(flight));
        var expected = condition.Value;

        if (field.DataType == RuleFieldDataType.Bool)
        {
            actual = NormalizeBool(actual);
            expected = NormalizeBool(expected);
        }

        return condition.Operator switch
        {
            ConditionOperator.IsNull => string.IsNullOrEmpty(actual),
            ConditionOperator.IsNotNull => !string.IsNullOrEmpty(actual),
            ConditionOperator.Equals => Eq(actual, expected),
            ConditionOperator.NotEquals => !Eq(actual, expected),
            ConditionOperator.StartsWith => actual is not null && expected is not null
                && actual.StartsWith(expected, StringComparison.OrdinalIgnoreCase),
            ConditionOperator.Contains => actual is not null && expected is not null
                && actual.Contains(expected, StringComparison.OrdinalIgnoreCase),
            ConditionOperator.In => actual is not null && SplitList(expected)
                .Any(v => string.Equals(v, actual, StringComparison.OrdinalIgnoreCase)),
            ConditionOperator.GreaterThan => Compare(actual, expected, field.DataType) > 0,
            ConditionOperator.LessThan => Compare(actual, expected, field.DataType) < 0,
            _ => false
        };
    }

    private static void ApplyAction(RuleAction action, Flight flight, FlightRuleFieldCatalog catalog, ResourceDefaults resourceDefaults)
    {
        if (action.ActionType == RuleActionType.RaiseError)
            throw new RuleValidationException(action.Value ?? "A rule blocked this save.");

        var field = catalog.Find(action.TargetField);
        if (field?.Setter is null) return;

        var newValue = action.ActionType == RuleActionType.AppendField
            ? Append(ToStringValue(field.Getter(flight)), action.Value)
            : action.Value;

        field.Setter(flight, newValue, action.MatchValue, resourceDefaults);
    }

    private static string? Append(string? current, string? addition)
    {
        if (string.IsNullOrEmpty(addition)) return current;
        if (string.IsNullOrEmpty(current)) return addition;

        var parts = current.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Any(p => string.Equals(p, addition.Trim(), StringComparison.OrdinalIgnoreCase)))
            return current;

        return current + ", " + addition;
    }

    private static bool Eq(string? a, string? b)
        => string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.OrdinalIgnoreCase);

    // Airport operators type booleans inconsistently ("Yes", "Y", "1", "True", ...) — normalize
    // to a canonical "true"/"false" before comparing so any of those spellings match.
    internal static string? NormalizeBool(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "true" or "1" or "yes" or "y" => "true",
            "false" or "0" or "no" or "n" => "false",
            _ => value
        };

    private static string[] SplitList(string? value)
        => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static int Compare(string? a, string? b, RuleFieldDataType dataType)
    {
        if (dataType is RuleFieldDataType.Int or RuleFieldDataType.Decimal
            && decimal.TryParse(a, NumberStyles.Any, CultureInfo.InvariantCulture, out var na)
            && decimal.TryParse(b, NumberStyles.Any, CultureInfo.InvariantCulture, out var nb))
            return na.CompareTo(nb);

        return string.Compare(a ?? string.Empty, b ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static string? ToStringValue(object? value)
        => value is null ? null : Convert.ToString(value, CultureInfo.InvariantCulture);
}
