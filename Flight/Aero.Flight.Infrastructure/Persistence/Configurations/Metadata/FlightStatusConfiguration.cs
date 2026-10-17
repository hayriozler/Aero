using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class FlightStatusConfiguration : IEntityTypeConfiguration<FlightStatus>
{
    public void Configure(EntityTypeBuilder<FlightStatus> builder)
    {        
        builder.ToTable("FlightStatus", "metadata");
        builder.HasKey(f => f.Code);
        builder.Property(f => f.Code).IsRequired().HasMaxLength(5).HasColumnType("citext");
        builder.Property(f => f.Description).HasMaxLength(50);
    }
}
