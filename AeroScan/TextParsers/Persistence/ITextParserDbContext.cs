using IataText.Parser.Entities;
using Microsoft.EntityFrameworkCore;

namespace IataText.Parser.Persistence;

public interface ITextParserDbContext
{
    DbSet<TextMessage> TextMessagesSet { get; }
    DbSet<TextMessageError> TextMessageErrorSet { get; }
    DbSet<ElementError> TextMessageElementErrorSet { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}