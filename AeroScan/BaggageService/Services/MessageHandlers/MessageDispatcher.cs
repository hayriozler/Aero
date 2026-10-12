using IataText.Parser.Entities;
using IataText.Parser.Parsers;
using IataText.Parser.Parsers.Messages;
using IataText.Parser.Persistence;
using Infrastructure.Persistence;

namespace BaggageService.Services.MessageHandlers;

public sealed class MessageDispatcher<TDto>(
    MessageBase<TDto> parser,
    IMessageResultHandler<TDto> handler) : IMessageParserDispatcher
{
    public async Task DispatchAsync(TextMessage message, ITextParserDbContext textParserDbContext, 
        IDataContext dataContext,
        CancellationToken cancellationToken)
    {
        var dto = parser.ProcessMessage(message, textParserDbContext);
        if (dto is not null)
            await handler.HandleAsync(message.Id, dto, dataContext, cancellationToken);
    }
}
