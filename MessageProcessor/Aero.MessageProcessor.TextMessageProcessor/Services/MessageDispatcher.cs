using Aero.MessageProcessor.TextMessageProcessor.Entities;
using Aero.MessageProcessor.TextMessageProcessor.Parsers;
using Aero.MessageProcessor.TextMessageProcessor.Parsers.Messages;
using Aero.MessageProcessor.TextMessageProcessor.Persistence;
using Aero.Shared.Infrastructure.Persistence;

namespace Aero.MessageProcessor.TextMessageProcessor.Services;

public sealed class MessageDispatcher<TDto>(
    MessageBase<TDto> parser,
    IMessageResultHandler<TDto> handler) : IMessageParserDispatcher
{
    public async Task DispatchAsync(TextMessage message, ITextMessageProcessorDbContext textParserDbContext,
        IDataContext dataContext,
        CancellationToken cancellationToken)
    {
        var dto = parser.ProcessMessage(message, textParserDbContext);
        if (dto is not null)
            await handler.HandleAsync(message.Id, dto, textParserDbContext, cancellationToken);
    }
}
