using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops.Passengers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Passengers;

internal sealed class TransferPassengerConfiguration : IEntityTypeConfiguration<OpsTransferPassenger>
{
    public void Configure(EntityTypeBuilder<OpsTransferPassenger> builder)
    {
        builder.ToTable("TransferPassenger", "ops");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSequence("FlightResourceSequence");
    

        builder.Property(t => t.ArrFlightNumber).IsRequired().HasMaxLength(10);
        builder.Property(t => t.DepFlightNumber).IsRequired().HasMaxLength(10);
        builder.Property(t => t.PassengerNames).IsRequired().HasMaxLength(500);
        builder.Property(t => t.DepDestination).HasMaxLength(3);
        builder.Property(t => t.BookingClass).HasMaxLength(10);
        builder.Property(t => t.BagWeight).HasPrecision(18, 4);
        builder.Property(t => t.Comment).HasMaxLength(500);

        builder.HasOne<Airport>().WithMany().HasForeignKey(t => t.DepDestination);

        AuditConfiguration.Apply<OpsTransferPassenger, long>(builder);
    }
}
