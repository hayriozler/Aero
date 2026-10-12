using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Region", "metadata");
        builder.HasKey(s => s.Code);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(10).HasColumnType("citext");
        builder.Property(s => s.Description).HasMaxLength(200);
    
    }
}