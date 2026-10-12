using Domain.Aggregates.Flights;
using Domain.Aggregates.Reconciliations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class ArrivalFlightReconciliationConfiguration
    : IEntityTypeConfiguration<ArrivalFlightReconciliation>
{
    public void Configure(EntityTypeBuilder<ArrivalFlightReconciliation> builder)
    {
        builder.ToTable("ArrivalFlightReconciliations", "bags");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn();

        builder.Property(r => r.FlightId).IsRequired();
        builder.Property(r => r.SnapshotAt).IsRequired();
        builder.Property(r => r.IsFinal).IsRequired();

        builder.Property(r => r.ExpectedBagCount);
        builder.Property(r => r.UnloadedBagCount);
        builder.Property(r => r.RemainingOnAircraftBagCount);
        builder.Property(r => r.ToBeltBagCount);
        builder.Property(r => r.DeliveredBagCount);
        builder.Property(r => r.TransferBagCount);
        builder.Property(r => r.MissingBagCount);
        builder.Property(r => r.UnknownBagCount);
        builder.Property(r => r.RushBagCount);

        builder.HasOne<ArrivalFlight>()
            .WithMany(f => f.ReconciliationRecords)
            .HasForeignKey(r => r.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.FlightId).IsUnique();
    }
}
