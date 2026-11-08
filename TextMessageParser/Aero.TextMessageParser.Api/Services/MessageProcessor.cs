using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Persistence;

namespace Aero.TextMessageParser.Api.Services;

public sealed class MessageProcessor<TResult>(
    MessageBase<TResult> parser,
    TextMessageParserDbContext dbContext) : IMessageProcessor
{
    public Task<bool> ProcessAsync(TextMessage message, CancellationToken cancellationToken)
    {
        var result = parser.ProcessMessage(message, dbContext);
        return Task.FromResult(result is not null);
    }
}
