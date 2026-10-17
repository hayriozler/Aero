using Aero.Flights.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations;

internal static class AuditConfiguration
{
    internal static void Apply<TEntity, TKey>(EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditBase<TKey>
        where TKey : notnull
    {
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
        builder.Property(e => e.UpdatedAt);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);
    }
}
