using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class TimezoneConfiguration : IEntityTypeConfiguration<Timezone>
{
    public void Configure(EntityTypeBuilder<Timezone> builder)
    {
        builder.ToTable("Timezone", "metadata");
        builder.HasKey(t => t.Code);
        builder.Property(t => t.Code).IsRequired().HasMaxLength(50).HasColumnType("citext");
        builder.Property(t => t.Description).IsRequired().HasMaxLength(100);
    }
}
