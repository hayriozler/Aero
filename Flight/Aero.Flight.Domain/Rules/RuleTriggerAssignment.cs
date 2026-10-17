namespace Aero.Flights.Domain.Rules;

public sealed class RuleTriggerAssignment
{
    public int Id { get; private set; }
    public int RuleId { get; private set; }
    public RuleTrigger Trigger { get; private set; }

    private RuleTriggerAssignment() { }

    private RuleTriggerAssignment(RuleTrigger trigger) => Trigger = trigger;

    public static RuleTriggerAssignment Create(RuleTrigger trigger) => new(trigger);
}
