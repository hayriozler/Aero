namespace AeroFlight.Domain.Metadatas;

// Lets resource bulk-status handlers resolve which status code stamps BeginActual/EndActual from
// data instead of hardcoding per resource kind (Gate/Counter use "O", Belt uses "S", etc.).
public interface IResourceStatus
{
    string Code { get; }
    bool IsDefault { get; }
    bool IsOpenValue { get; }
    bool IsCloseValue { get; }
}
