using Aero.MessageProcessor.TextMessageProcessor.Entities;
using Aero.MessageProcessor.TextMessageProcessor.Persistence;
using Aero.Shared.Infrastructure.Persistence;

namespace Aero.MessageProcessor.TextMessageProcessor.Parsers;

public interface IMessageParserDispatcher
{
    Task DispatchAsync(TextMessage message, ITextMessageProcessorDbContext textMessageProcessorDbContext, IDataContext dataContext, CancellationToken cancellationToken);
}

