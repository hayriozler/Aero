using AeroFlight.Domain.Ops.Passengers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Passengers;

internal sealed class ArrivalPassengerConfiguration : IEntityTypeConfiguration<OpsArrivalPassenger>
{
    public void Configure(EntityTypeBuilder<OpsArrivalPassenger> builder)
    {
        builder.ToTable("ArrivalPassenger", "ops");
        builder.Property(p => p.Id).UseSequence("FlightResourceSequence");
    }
}
