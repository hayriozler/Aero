using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class DeBoardingStatusConfiguration : IEntityTypeConfiguration<DeBoardingStatus>
{
    public void Configure(EntityTypeBuilder<DeBoardingStatus> builder)
    {
        builder.ToTable("DeBoardingStatus", "metadata");
        builder.HasKey(b => b.Code);
        builder.Property(b => b.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(b => b.Description).HasMaxLength(200);
    }
}
