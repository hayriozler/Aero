using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class ParkPositionTypeConfiguration : IEntityTypeConfiguration<ParkPositionType>
{
    public void Configure(EntityTypeBuilder<ParkPositionType> builder)
    {
        builder.ToTable("ParkPositionType", "metadata");
        builder.HasKey(g => g.Code);

        builder.Property(g => g.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(g => g.Description).IsRequired().HasMaxLength(200);
    }
}