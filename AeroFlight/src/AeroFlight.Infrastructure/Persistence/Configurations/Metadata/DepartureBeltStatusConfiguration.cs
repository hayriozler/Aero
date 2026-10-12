using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class DepartureBeltStatusConfiguration : IEntityTypeConfiguration<DepartureBeltStatus>
{
    public void Configure(EntityTypeBuilder<DepartureBeltStatus> builder)
    {
        builder.ToTable("DepartureBeltStatus", "metadata");
        builder.HasKey(b => b.Code);
        builder.Property(b => b.Code).IsRequired().HasMaxLength(10).HasColumnType("citext");
        builder.Property(b => b.Description).HasMaxLength(200);
        builder.Property(b => b.IsDefault).HasDefaultValue(false);
        builder.Property(b => b.IsOpenValue).HasDefaultValue(false);
        builder.Property(b => b.IsCloseValue).HasDefaultValue(false);
    }
}
