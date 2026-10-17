namespace Aero.Flights.Domain.Metadatas;

// Read-only mirror of FlightRuleFieldCatalog/RuleActionType, resynced from code on every seed run
// (see RuleCatalogSeeder) so it can never drift from what the rule engine actually executes.
// FlightRuleFieldCatalog's Getter/Setter delegates remain the only thing that runs a rule action —
// these rows exist purely so the metadata query (and anyone querying the DB directly) can use a
// real Metadata-schema table instead of an in-memory array.
public sealed class RuleActionTypeMetadata
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private RuleActionTypeMetadata() { }
}

// Table is "RuleTriggerOption" (see RuleTriggerMetadataConfiguration), not "RuleTrigger" — that
// name is already ops."RuleTrigger" (RuleTriggerAssignment, which triggers an actual Rule
// listens for), and EF's constraint naming isn't schema-qualified. This one is just the option
// list (code + friendly label) for the Triggers multiselect.
public sealed class RuleTriggerMetadata
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private RuleTriggerMetadata() { }
}

public sealed class RuleFieldMetadata
{
    public string Key { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public string DataType { get; private set; } = string.Empty;
    public bool UsableInCondition { get; private set; }
    public bool UsableInAction { get; private set; }
    public bool SupportsMatchValue { get; private set; }
    public string? MatchValueLabel { get; private set; }

    private RuleFieldMetadata() { }
}

public sealed class RuleFieldTriggerMetadata
{
    public string RuleFieldKey { get; private set; } = string.Empty;
    public string Trigger { get; private set; } = string.Empty;

    private RuleFieldTriggerMetadata() { }
}

public sealed class RuleFieldActionTypeMetadata
{
    public string RuleFieldKey { get; private set; } = string.Empty;
    public string ActionTypeCode { get; private set; } = string.Empty;

    private RuleFieldActionTypeMetadata() { }
}
