using AeroFlight.Domain.Ops.MovementMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.MovementMessages;

internal sealed class DepartureMovementMessageConfiguration : IEntityTypeConfiguration<OpsDepartureMovementMessage>
{
    public void Configure(EntityTypeBuilder<OpsDepartureMovementMessage> builder)
    {
        builder.ToTable("DepartureMovementMessage", "ops");
        builder.Property(m => m.Id).UseSequence("FlightResourceSequence");
    }
}
