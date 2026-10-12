using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AirportImageConfiguration : IEntityTypeConfiguration<AirportImage>
{
    public void Configure(EntityTypeBuilder<AirportImage> builder)
    {
        builder.ToTable("AirportImage", "metadata");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.AirportCode).IsRequired().HasMaxLength(3).HasColumnType("citext");
        builder.Property(i => i.ImageData).IsRequired();
        builder.Property(i => i.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(i => i.DisplayOrder).IsRequired();

        builder.HasOne<Airport>().WithMany().HasForeignKey(i => i.AirportCode).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.AirportCode);
    }
}
