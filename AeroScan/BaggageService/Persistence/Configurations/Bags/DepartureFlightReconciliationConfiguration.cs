using Domain.Aggregates.Flights;
using Domain.Aggregates.Reconciliations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class DepartureFlightReconciliationConfiguration
    : IEntityTypeConfiguration<DepartureFlightReconciliation>
{
    public void Configure(EntityTypeBuilder<DepartureFlightReconciliation> builder)
    {
        builder.ToTable("DepartureFlightReconciliations", "bags");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn();

        builder.Property(r => r.FlightId).IsRequired();
        builder.Property(r => r.SnapshotAt).IsRequired();
        builder.Property(r => r.IsFinal).IsRequired();

        builder.Property(r => r.ExpectedBagCount);
        builder.Property(r => r.LoadedBagCount);
        builder.Property(r => r.OffloadedCount);
        builder.Property(r => r.ToBeOffloadedCount);
        builder.Property(r => r.WaitingToLoadBagCount);
        builder.Property(r => r.MissingBagCount);
        builder.Property(r => r.ReconciledBagCount);
        builder.Property(r => r.ForceLoadedBagCount);
        builder.Property(r => r.OnwardBagCount);
        builder.Property(r => r.TransferLoadedBagCount);
        builder.Property(r => r.TransferMissingBagCount);
        builder.Property(r => r.NotBoardedPassengerBagCount);
        builder.Property(r => r.RushBagCount);
        builder.Property(r => r.PriorityBagCount);

        builder.HasOne<DepartureFlight>()
            .WithMany(f => f.ReconciliationRecords)
            .HasForeignKey(r => r.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.FlightId).IsUnique();
    }
}
