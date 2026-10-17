using Aero.Flights.Domain.Ops.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Services;

internal sealed class DepartureServiceConfiguration : IEntityTypeConfiguration<OpsDepartureService>
{
    public void Configure(EntityTypeBuilder<OpsDepartureService> builder)
    {
        builder.ToTable("DepartureService", "ops");
        builder.Property(s => s.Id).UseSequence("FlightResourceSequence");
    }
}
