using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CounterStatusConfiguration : IEntityTypeConfiguration<CounterStatus>
{
    public void Configure(EntityTypeBuilder<CounterStatus> builder)
    {
        builder.ToTable("CounterStatus", "metadata");
        builder.HasKey(c => c.Code);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(c => c.Description).HasMaxLength(200);
        builder.Property(c => c.IsDefault).HasDefaultValue(false);
        builder.Property(c => c.IsOpenValue).HasDefaultValue(false);
        builder.Property(c => c.IsCloseValue).HasDefaultValue(false);
    }
}
