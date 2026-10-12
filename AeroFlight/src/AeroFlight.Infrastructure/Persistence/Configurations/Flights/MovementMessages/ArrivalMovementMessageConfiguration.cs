using AeroFlight.Domain.Ops.MovementMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.MovementMessages;

internal sealed class ArrivalMovementMessageConfiguration : IEntityTypeConfiguration<OpsArrivalMovementMessage>
{
    public void Configure(EntityTypeBuilder<OpsArrivalMovementMessage> builder)
    {
        builder.ToTable("ArrivalMovementMessage", "ops");
        builder.Property(m => m.Id).UseSequence("FlightResourceSequence");
    }
}
