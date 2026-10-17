using Aero.Flights.Domain.Ops.Delays;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Delays;

internal sealed class DepartureDelayConfiguration : IEntityTypeConfiguration<OpsDepartureDelay>
{
    public void Configure(EntityTypeBuilder<OpsDepartureDelay> builder)
    {
        builder.ToTable("DepartureDelay", "ops");
        builder.Property(d => d.Id).UseSequence("FlightResourceSequence");
    }
}
