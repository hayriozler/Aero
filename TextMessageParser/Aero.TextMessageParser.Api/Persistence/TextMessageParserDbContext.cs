using Aero.Shared.Infrastructure.Persistence;
using Aero.TextMessageParser.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Persistence;

public class TextMessageParserDbContext(DbContextOptions<TextMessageParserDbContext> options)
    : DataStoreContext(options)
{
    public DbSet<TextMessage> TextMessagesSet => Set<TextMessage>();
    public DbSet<TextMessageError> TextMessageErrorSet => Set<TextMessageError>();
    public DbSet<ElementError> TextMessageElementErrorSet => Set<ElementError>();

}
