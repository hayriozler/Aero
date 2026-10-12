using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AirlineContactConfiguration : IEntityTypeConfiguration<AirlineContact>
{
    public void Configure(EntityTypeBuilder<AirlineContact> builder)
    {
        builder.ToTable("AirlineContact", "metadata");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.AirlineCode).IsRequired().HasMaxLength(3).HasColumnType("citext");
        builder.Property(c => c.ContactType).HasConversion<string>().IsRequired().HasMaxLength(10);
        builder.Property(c => c.Value).IsRequired().HasMaxLength(100);

        builder.HasOne<Airline>().WithMany().HasForeignKey(c => c.AirlineCode).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.AirlineCode);
    }
}
