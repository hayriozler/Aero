using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class GateStatusConfiguration : IEntityTypeConfiguration<GateStatus>
{
    public void Configure(EntityTypeBuilder<GateStatus> builder)
    {
        builder.ToTable("GateStatus", "metadata");
        builder.HasKey(g => g.Code);
        builder.Property(g => g.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(g => g.Description).HasMaxLength(200);
        builder.Property(g => g.IsDefault).HasDefaultValue(false);
        builder.Property(g => g.IsOpenValue).HasDefaultValue(false);
        builder.Property(g => g.IsCloseValue).HasDefaultValue(false);
    }
}
