using Aero.MessageProcessor.TextMessageProcessor.Entities;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.MessageProcessor.TextMessageProcessor.Persistence;

public class TextMessageProcessorDbContext(DbContextOptions<TextMessageProcessorDbContext> options)
    : DataStoreContext(options), ITextMessageProcessorDbContext
{
    public DbSet<TextMessage> TextMessagesSet => Set<TextMessage>();
    public DbSet<TextMessageError> TextMessageErrorSet => Set<TextMessageError>();
    public DbSet<ElementError> TextMessageElementErrorSet => Set<ElementError>();

}
