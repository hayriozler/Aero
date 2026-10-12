using Aero.MessageProcessor.TextMessageProcessor.Persistence;

namespace Aero.MessageProcessor.TextMessageProcessor.Services;

public interface IMessageResultHandler<TDto>
{
    Task HandleAsync(long messageId, TDto dto, ITextMessageProcessorDbContext dbContext, CancellationToken cancellationToken);
}
