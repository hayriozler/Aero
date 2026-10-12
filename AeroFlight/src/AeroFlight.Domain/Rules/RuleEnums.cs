namespace AeroFlight.Domain.Rules;

public enum RuleTrigger
{
    // Direction-agnostic — fires for both ArrivalFlight and DepartureFlight, in addition to the
    // direction-specific trigger below, so a rule that doesn't care which one it is only needs
    // to be defined once.
    FlightCreated,
    FlightUpdated,
    FlightDeleted,
    DepartureCreated,
    DepartureDeleted,
    DepartureUpdated,
    CounterCreated, 
    CounterDeleted,
    CounterUpdated,
    GateCreated, 
    GateDeleted, 
    GateUpdated,
    ParkPositionCreated,
    ParkPositionDeleted,
    ParkPositionUpdated,
    DepartureBeltCreated, 
    DepartureBeltDeleted, 
    DepartureBeltUpdated,
    ArrivalCreated,
    ArrivalDeleted,
    ArrivalUpdated,
    ArrivalBeltCreated, 
    ArrivalBeltDeleted, 
    ArrivalBeltUpdated,
}

public enum ConditionOperator
{
    Equals,
    NotEquals,
    In,
    GreaterThan,
    LessThan,
    StartsWith,
    Contains,
    IsNull,
    IsNotNull
}

// How a RuleConditionGroup's own conditions combine: And requires all of them, Or requires any
// one. Groups themselves are always ANDed together at the rule level — see RuleConditionGroup.
public enum ConditionLogicalOperator
{
    And,
    Or
}

public enum RuleActionType
{
    SetField,
    AppendField,
    RequireResourceCount,
    Finance,
    RaiseError
}

public enum RuleFieldDataType
{
    String,
    Int,
    Decimal,
    Bool,
    Time
}
