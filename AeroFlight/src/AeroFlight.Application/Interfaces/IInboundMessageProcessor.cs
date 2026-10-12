namespace AeroFlight.Application.Interfaces;

public interface IInboundMessageProcessor
{
    string MessageType { get; }
    Task ProcessAsync(string payload, CancellationToken ct);
}
