namespace AeroFlight.Domain.Rules;

public sealed class RuleAction
{
    public int Id { get; private set; }
    public int RuleId { get; private set; }
    public RuleActionType ActionType { get; private set; }
    public string TargetField { get; private set; } = string.Empty;
    public string? Value { get; private set; }
    // Restricts the action to just the item(s) matching this key within a multi-instance target
    // (e.g. a ground service code, so "GPU" and "SECURITY" can be billed to different payers by
    // two separate actions on the same field) — null applies the action to every item, same as
    // before this existed. Only meaningful for fields where FlightRuleField.SupportsMatchValue.
    public string? MatchValue { get; private set; }
    public int Order { get; private set; }

    private RuleAction() { }

    private RuleAction(RuleActionType actionType, string targetField, string? value, string? matchValue, int order)
    {
        ActionType = actionType;
        TargetField = targetField;
        Value = value;
        MatchValue = matchValue;
        Order = order;
    }

    public static RuleAction Create(RuleActionType actionType, string targetField, string? value, string? matchValue, int order)
        => new(actionType, targetField, value, matchValue, order);
}
