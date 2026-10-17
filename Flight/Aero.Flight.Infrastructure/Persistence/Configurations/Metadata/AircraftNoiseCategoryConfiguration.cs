using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AircraftNoiseCategoryConfiguration : IEntityTypeConfiguration<AircraftNoiseCategory>
{
    public void Configure(EntityTypeBuilder<AircraftNoiseCategory> builder)
    {
        builder.ToTable("AircraftNoiseCategory", "metadata");
        builder.HasKey(n => n.Code);
        builder.Property(n => n.Code).IsRequired().HasMaxLength(10).HasColumnType("citext");
        builder.Property(n => n.Description).IsRequired().HasMaxLength(200);
    }
}
