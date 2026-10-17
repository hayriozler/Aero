using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class FlightTypeConfiguration : IEntityTypeConfiguration<FlightType>
{
    public void Configure(EntityTypeBuilder<FlightType> builder)
    {
        builder.ToTable("FlightType", "metadata");
        builder.HasKey(f => f.Code);
        builder.Property(f => f.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(f => f.Description).HasMaxLength(100);
        builder.Property(f => f.DisplayCode).HasMaxLength(1);
    }
}
