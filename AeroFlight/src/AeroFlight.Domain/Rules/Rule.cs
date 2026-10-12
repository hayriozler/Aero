using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Rules;

public sealed class Rule : AuditBase<int>
{
    private readonly List<RuleTriggerAssignment> _triggers = [];
    private readonly List<RuleConditionGroup> _conditionGroups = [];
    private readonly List<RuleAction> _actions = [];

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsEnabled { get; private set; }
    public bool StopOnMatch { get; private set; }
    public IReadOnlyCollection<RuleTriggerAssignment> Triggers => _triggers;
    public IReadOnlyCollection<RuleConditionGroup> ConditionGroups => _conditionGroups;
    public IReadOnlyCollection<RuleAction> Actions => _actions;

    private Rule() { }

    private Rule(string name, string? description, IEnumerable<RuleTrigger> triggers, bool isEnabled, bool stopOnMatch)
    {
        Name = name;
        Description = description;
        IsEnabled = isEnabled;
        StopOnMatch = stopOnMatch;
        SetTriggers(triggers);
    }

    public static Rule Create(string name, string? description, IEnumerable<RuleTrigger> triggers, bool isEnabled, bool stopOnMatch)
        => new(name, description, triggers, isEnabled, stopOnMatch);

    public void Update(string name, string? description, IEnumerable<RuleTrigger> triggers, bool isEnabled, bool stopOnMatch)
    {
        Name = name;
        Description = description;
        IsEnabled = isEnabled;
        StopOnMatch = stopOnMatch;
        SetTriggers(triggers);
    }

    public void SetEnabled(bool isEnabled) => IsEnabled = isEnabled;

    public RuleConditionGroup AddConditionGroup(ConditionLogicalOperator @operator)
    {
        var group = RuleConditionGroup.Create(_conditionGroups.Count, @operator);
        _conditionGroups.Add(group);
        return group;
    }

    public void AddAction(RuleActionType actionType, string targetField, string? value, string? matchValue, int order)
        => _actions.Add(RuleAction.Create(actionType, targetField, value, matchValue, order));

    public void ClearConditionGroups() => _conditionGroups.Clear();

    public void ClearActions() => _actions.Clear();

    private void SetTriggers(IEnumerable<RuleTrigger> triggers)
    {
        _triggers.Clear();
        foreach (var trigger in triggers.Distinct())
            _triggers.Add(RuleTriggerAssignment.Create(trigger));
    }
}
