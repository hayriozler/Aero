using Aero.MessageProcessor.TextMessageProcessor.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aero.MessageProcessor.TextMessageProcessor.Persistence;

public interface ITextMessageProcessorDbContext
{
    DbSet<TextMessage> TextMessagesSet { get; }
    DbSet<TextMessageError> TextMessageErrorSet { get; }
    DbSet<ElementError> TextMessageElementErrorSet { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}