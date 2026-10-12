namespace Aero.MessageProcessor.TextMessageProcessor.Contracts;

public interface IMessageHandler
{
    string MessageName  { get; }
    string EndIdentifier { get; }
}
