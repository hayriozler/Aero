using Aero.Flights.Contracts.Rules;
using Aero.Flights.Domain.Rules;
using FluentValidation;

namespace Aero.Flights.Application.Rules;

public sealed class RuleRequestValidator : AbstractValidator<RuleRequest>
{
    private readonly FlightRuleFieldCatalog _catalog;

    public RuleRequestValidator(FlightRuleFieldCatalog catalog)
    {
        _catalog = catalog;

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);

        RuleFor(x => x.Triggers).NotEmpty().WithMessage("At least one trigger is required.");

        RuleForEach(x => x.Triggers)
            .Must(BeEnum<RuleTrigger>)
            .WithMessage((_, trigger) => $"'{trigger}' is not a valid trigger.");

        RuleForEach(x => x.ConditionGroups).ChildRules(g =>
        {
            g.RuleFor(x => x.Operator)
                .Must(BeEnum<ConditionLogicalOperator>)
                .WithMessage(x => $"'{x.Operator}' is not a valid logical operator.");

            g.RuleForEach(x => x.Conditions).ChildRules(c =>
            {
                c.RuleFor(x => x.Field).NotEmpty();
                c.RuleFor(x => x.Operator)
                    .Must(BeEnum<ConditionOperator>)
                    .WithMessage(x => $"'{x.Operator}' is not a valid operator.");
            });
        });

        RuleForEach(x => x.Actions).ChildRules(a =>
        {
            a.RuleFor(x => x.TargetField).NotEmpty();
            a.RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
            a.RuleFor(x => x.ActionType)
                .Must(BeEnum<RuleActionType>)
                .WithMessage(x => $"'{x.ActionType}' is not a valid action type.");
        });

        RuleFor(x => x).Custom(ValidateRule);
    }

    private void ValidateRule(RuleRequest request, ValidationContext<RuleRequest> ctx)
    {
        var triggers = request.Triggers
            .Select(t => Enum.TryParse<RuleTrigger>(t, ignoreCase: true, out var parsed) ? parsed : (RuleTrigger?)null)
            .Where(t => t is not null)
            .Select(t => t!.Value)
            .ToList();
        var hasTrigger = triggers.Count > 0;

        var allConditions = request.ConditionGroups.SelectMany(g => g.Conditions).ToList();

        foreach (var condition in allConditions)
        {
            var field = _catalog.Find(condition.Field);
            if (field is null)
            {
                ctx.AddFailure(nameof(request.ConditionGroups), $"Unknown condition field '{condition.Field}'.");
                continue;
            }

            if (!field.UsableInCondition)
                ctx.AddFailure(nameof(request.ConditionGroups), $"Field '{condition.Field}' cannot be used in a condition.");

            var missingTriggers = triggers.Where(t => !field.Triggers.Contains(t)).ToList();
            if (hasTrigger && missingTriggers.Count > 0)
                ctx.AddFailure(nameof(request.ConditionGroups),
                    $"Field '{condition.Field}' is not available for trigger(s) '{string.Join(", ", missingTriggers)}'.");

            if (Enum.TryParse<ConditionOperator>(condition.Operator, ignoreCase: true, out var op)
                && op is not (ConditionOperator.IsNull or ConditionOperator.IsNotNull)
                && string.IsNullOrWhiteSpace(condition.Value))
                ctx.AddFailure(nameof(request.ConditionGroups), $"Operator '{op}' on field '{condition.Field}' requires a value.");
        }

        foreach (var action in request.Actions)
        {
            var field = _catalog.Find(action.TargetField);
            if (field is null)
            {
                ctx.AddFailure(nameof(request.Actions), $"Unknown target field '{action.TargetField}'.");
                continue;
            }

            if (!field.UsableInAction)
                ctx.AddFailure(nameof(request.Actions), $"Field '{action.TargetField}' cannot be used in an action.");

            var missingTriggers = triggers.Where(t => !field.Triggers.Contains(t)).ToList();
            if (hasTrigger && missingTriggers.Count > 0)
                ctx.AddFailure(nameof(request.Actions),
                    $"Field '{action.TargetField}' is not available for trigger(s) '{string.Join(", ", missingTriggers)}'.");

            if (string.IsNullOrWhiteSpace(action.Value))
                ctx.AddFailure(nameof(request.Actions), $"Action on field '{action.TargetField}' requires a value.");

            if (!string.IsNullOrWhiteSpace(action.MatchValue) && !field.SupportsMatchValue)
                ctx.AddFailure(nameof(request.Actions), $"Field '{action.TargetField}' does not support a match value.");
        }

        var conflictingSet = request.Actions
            .Where(a => Enum.TryParse<RuleActionType>(a.ActionType, ignoreCase: true, out var t) && t == RuleActionType.SetField)
            .GroupBy(a => a.TargetField, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (var field in conflictingSet)
            ctx.AddFailure(nameof(request.Actions), $"Field '{field}' is set by more than one action.");

        if (hasTrigger)
        {
            // A condition only unconditionally applies if it's the sole condition in its group, or
            // its group ANDs everything together — inside an Or group with other conditions, it's
            // one of several options, so an equality there isn't a hard requirement. Only equality
            // conditions that ARE hard requirements can genuinely contradict each other.
            var requiredEquals = request.ConditionGroups
                .Where(g => g.Conditions.Count == 1 || string.Equals(g.Operator, "And", StringComparison.OrdinalIgnoreCase))
                .SelectMany(g => g.Conditions)
                .Where(c => Enum.TryParse<ConditionOperator>(c.Operator, ignoreCase: true, out var op) && op == ConditionOperator.Equals);

            var contradictoryEquals = requiredEquals
                .GroupBy(c => c.Field, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
                .Select(g => g.Key);

            foreach (var field in contradictoryEquals)
                ctx.AddFailure(nameof(request.ConditionGroups), $"Field '{field}' has contradictory equality conditions.");
        }
    }

    private static bool BeEnum<TEnum>(string? value) where TEnum : struct, Enum
        => !string.IsNullOrWhiteSpace(value)
           && !char.IsDigit(value[0])
           && Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed)
           && Enum.IsDefined(parsed);
}
