using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights;

internal sealed class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.HasKey(f => f.Id);
        AuditConfiguration.Apply<Flight, long>(builder);

        builder.Property(f => f.AirlineCode).IsRequired().HasMaxLength(3);
        builder.Property(f => f.FlightNumber).IsRequired().HasMaxLength(10);
        builder.Property(f => f.CallSign).HasMaxLength(8);
        builder.Property(f => f.FlightTypeCode).IsRequired().HasMaxLength(1);
        builder.Property(f => f.Status)
            .HasColumnName("StatusCode")
            .HasMaxLength(5);
        builder.Property(f => f.CountryType).HasMaxLength(1);
        builder.Property(f => f.AircraftCode).HasMaxLength(10);
        builder.Property(f => f.AircraftTypeCode).HasMaxLength(5);
        builder.Property(f => f.OperatingAirlineCode).HasMaxLength(3);
        builder.Property(f => f.AircraftOwner).HasMaxLength(50);
        builder.Property(f => f.ParkPositionCode).HasMaxLength(10);
        builder.Property(f => f.CurrentParkPositionCode).HasMaxLength(10);
        builder.Property(f => f.TerminalCode).HasMaxLength(10);
        builder.Property(f => f.HandlingAgentCode).HasMaxLength(10);
        builder.Property(f => f.Comment).HasMaxLength(250);

        // Cascade is EF's default for a required FK — Restrict here on purpose, so deleting an
        // Airline/FlightType still in use by flights fails loudly instead of silently wiping
        // flight history. Blocked ahead of time by MetadataDeleteCommandHandler's usage guard
        // (see AirlineUsageGuard/FlightTypeUsageGuard); this is the DB-level backstop.
        builder.HasOne<Airline>().WithMany().HasForeignKey(f => f.AirlineCode).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Airline>().WithMany().HasForeignKey(f => f.OperatingAirlineCode);
        builder.HasOne<FlightType>().WithMany().HasForeignKey(f => f.FlightTypeCode).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CountryType>().WithMany().HasForeignKey(f => f.CountryType);
        builder.HasOne<Aircraft>().WithMany().HasForeignKey(f => f.AircraftCode);
        builder.HasOne<AircraftType>().WithMany().HasForeignKey(f => f.AircraftTypeCode);
        builder.HasOne<ParkPosition>().WithMany().HasForeignKey(f => f.ParkPositionCode);
        builder.HasOne<ParkPosition>().WithMany().HasForeignKey(f => f.CurrentParkPositionCode);
        builder.HasOne<Terminal>().WithMany().HasForeignKey(f => f.TerminalCode);        
    }
}
