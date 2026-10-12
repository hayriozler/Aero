using AeroFlight.Domain.Ops.Passengers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Passengers;

internal sealed class PassengerConfiguration : IEntityTypeConfiguration<OpsPassenger>
{
    public void Configure(EntityTypeBuilder<OpsPassenger> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FlightId).IsRequired();
        builder.Property(p => p.PassengerType).HasMaxLength(20);
        builder.Property(p => p.Count).IsRequired();
    }
}
