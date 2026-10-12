using Domain.Aggregates.Bags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class DepartureFlightPassengerConfiguration : IEntityTypeConfiguration<DepartureFlightPassenger>
{
    public void Configure(EntityTypeBuilder<DepartureFlightPassenger> builder)
    {
        builder.ToTable("DepartureFlightPassengers", "bags");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityColumn();
        builder.Property(p => p.FlightId).IsRequired();
        builder.Property(b => b.PassengerStatus).HasConversion<short>().IsRequired();
        builder.Property(p => p.SecurityNumber).HasMaxLength(5);
        builder.Property(p => p.SequenceNumber).HasMaxLength(5);
        builder.Property(p => p.Seat).HasMaxLength(8);
        builder.Property(p => p.PassengerName).HasMaxLength(55).IsRequired();
        builder.Property(p => p.Destination).HasMaxLength(3).IsRequired();
        builder.Property(p => p.IsOnward).IsRequired();

        builder.HasOne(p => p.Flight)
            .WithMany()
            .HasForeignKey(p => p.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.Bags);

        // Partial unique indexes: enforce uniqueness per available identifier
        builder.HasIndex(p => new { p.FlightId, p.SecurityNumber })
            .IsUnique()
            .HasFilter("\"SecurityNumber\" IS NOT NULL");

        builder.HasIndex(p => new { p.FlightId, p.SequenceNumber })
            .IsUnique()
            .HasFilter("\"SequenceNumber\" IS NOT NULL");

        builder.HasIndex(p => p.FlightId).IncludeProperties([nameof(DepartureFlightPassenger.Id)]);
    }
}
