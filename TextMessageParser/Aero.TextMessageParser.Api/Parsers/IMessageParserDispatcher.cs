using Aero.TextMessageParser.Api.Entities;

namespace Aero.TextMessageParser.Api.Parsers;

public interface IMessageParserDispatcher
{
    Task<bool> DispatchAsync(TextMessage message, CancellationToken cancellationToken);
}

