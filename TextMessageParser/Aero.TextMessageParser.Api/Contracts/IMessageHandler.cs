namespace Aero.TextMessageParser.Api.Contracts;

public interface IMessageHandler
{
    string MessageName  { get; }
    string EndIdentifier { get; }
}
