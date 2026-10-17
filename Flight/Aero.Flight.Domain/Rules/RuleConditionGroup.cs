namespace Aero.Flights.Domain.Rules;

// Groups let a rule express "(A OR B) AND C": each group's conditions are joined by its own
// single Operator (all AND, or all OR), and groups themselves are always ANDed together at the
// rule level — a two-level model (conjunction of groups, each a disjunction or conjunction
// internally) rather than arbitrary nested parentheses, which covers real rule shapes without
// the UI complexity of a full expression tree.
public sealed class RuleConditionGroup
{
    private readonly List<RuleCondition> _conditions = [];

    public int Id { get; private set; }
    public int RuleId { get; private set; }
    public int Order { get; private set; }
    public ConditionLogicalOperator Operator { get; private set; }
    public IReadOnlyCollection<RuleCondition> Conditions => _conditions;

    private RuleConditionGroup() { }

    private RuleConditionGroup(int order, ConditionLogicalOperator @operator)
    {
        Order = order;
        Operator = @operator;
    }

    public static RuleConditionGroup Create(int order, ConditionLogicalOperator @operator) => new(order, @operator);

    public void AddCondition(string field, ConditionOperator @operator, string? value)
        => _conditions.Add(RuleCondition.Create(field, @operator, value));
}
