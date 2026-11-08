using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Persistence;

namespace Aero.TextMessageParser.Api.Services;

public sealed class MessageProcessor<TResult>(
    MessageParser<TResult> parser,
    TextMessageParserDbContext dbContext) : IMessageProcessor
    where TResult : class
{
    public Task<bool> ProcessAsync(TextMessage message, CancellationToken cancellationToken)
    {
        var result = parser.ProcessMessage(message, dbContext);
        if (result is null)
            return Task.FromResult(false);

        var parsed = ParsedMessageFactory.Create(message, result);
        if (parsed is not null)
            dbContext.ParsedMessages.Add(parsed);
        return Task.FromResult(true);
    }
}
