using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AirlineLogoConfiguration : IEntityTypeConfiguration<AirlineLogo>
{
    public void Configure(EntityTypeBuilder<AirlineLogo> builder)
    {
        builder.ToTable("AirlineLogo", "metadata");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.AirlineCode).IsRequired().HasMaxLength(3).HasColumnType("citext");
        builder.Property(l => l.Name).IsRequired().HasMaxLength(50);
        builder.Property(l => l.ImageData).IsRequired();
        builder.Property(l => l.ContentType).IsRequired().HasMaxLength(100);

        builder.HasOne<Airline>().WithMany().HasForeignKey(l => l.AirlineCode).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.AirlineCode);
    }
}
