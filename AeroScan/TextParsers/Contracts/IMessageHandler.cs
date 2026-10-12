namespace IataText.Parser.Contracts;

public interface IMessageHandler
{
    string MessageName  { get; }
    string EndIdentifier { get; }
}
