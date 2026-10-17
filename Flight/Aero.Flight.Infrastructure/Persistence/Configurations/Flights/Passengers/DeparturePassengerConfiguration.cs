using Aero.Flights.Domain.Ops.Passengers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Passengers;

internal sealed class DeparturePassengerConfiguration : IEntityTypeConfiguration<OpsDeparturePassenger>
{
    public void Configure(EntityTypeBuilder<OpsDeparturePassenger> builder)
    {
        builder.ToTable("DeparturePassenger", "ops");
        builder.Property(p => p.Id).UseSequence("FlightResourceSequence");
    }
}
