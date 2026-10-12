namespace AeroFlight.Domain.Rules;

public sealed class RuleCondition
{
    public int Id { get; private set; }
    public int GroupId { get; private set; }
    public string Field { get; private set; } = string.Empty;
    public ConditionOperator Operator { get; private set; }
    public string? Value { get; private set; }

    private RuleCondition() { }

    private RuleCondition(string field, ConditionOperator @operator, string? value)
    {
        Field = field;
        Operator = @operator;
        Value = value;
    }

    public static RuleCondition Create(string field, ConditionOperator @operator, string? value) => new(field, @operator, value);
}
