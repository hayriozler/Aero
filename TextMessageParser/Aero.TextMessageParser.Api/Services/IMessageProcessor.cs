using Aero.TextMessageParser.Api.Entities;

namespace Aero.TextMessageParser.Api.Services;

public interface IMessageProcessor
{
    Task<bool> ProcessAsync(TextMessage message, CancellationToken cancellationToken);
}
