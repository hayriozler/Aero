namespace AeroFlight.Contracts.Rules;

public sealed record RuleConditionRequest(string Field, string Operator, string? Value);

public sealed record RuleConditionGroupRequest(string Operator, IReadOnlyList<RuleConditionRequest> Conditions);

public sealed record RuleActionRequest(string ActionType, string TargetField, string? Value, int Order, string? MatchValue = null);

public sealed record RuleRequest(
    string Name,
    string? Description,
    IReadOnlyList<string> Triggers,
    bool IsEnabled,
    bool StopOnMatch,
    IReadOnlyList<RuleConditionGroupRequest> ConditionGroups,
    IReadOnlyList<RuleActionRequest> Actions);

public sealed record RuleConditionResponse(int Id, string Field, string Operator, string? Value);

public sealed record RuleConditionGroupResponse(int Id, string Operator, IReadOnlyList<RuleConditionResponse> Conditions);

public sealed record RuleActionResponse(int Id, string ActionType, string TargetField, string? Value, int Order, string? MatchValue = null);

public sealed record RuleResponse(
    int Id,
    string Name,
    string? Description,
    IReadOnlyList<string> Triggers,
    bool IsEnabled,
    bool StopOnMatch,
    IReadOnlyList<RuleConditionGroupResponse> ConditionGroups,
    IReadOnlyList<RuleActionResponse> Actions,
    DateTimeOffset CreatedAt,
    string CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy);

public sealed record RuleSetEnabledRequest(bool IsEnabled);

public sealed record RuleOptionResponse(string Key, string Label);

public sealed record RuleOperatorOptionResponse(string Key, string Label, string Arity);

public sealed record RuleEnumValueResponse(string Value, string Label);

public sealed record RuleFieldOptionResponse(
    string Key,
    string Label,
    string DataType,
    IReadOnlyList<string> Triggers,
    bool UsableInCondition,
    bool UsableInAction,
    IReadOnlyList<RuleEnumValueResponse>? EnumValues,
    IReadOnlyList<string>? ActionTypes = null,
    bool SupportsMatchValue = false,
    string? MatchValueLabel = null);

public sealed record RuleMetadataResponse(
    IReadOnlyList<RuleOptionResponse> Triggers,
    IReadOnlyList<RuleOperatorOptionResponse> Operators,
    IReadOnlyList<RuleOptionResponse> ActionTypes,
    IReadOnlyList<RuleFieldOptionResponse> Fields);
