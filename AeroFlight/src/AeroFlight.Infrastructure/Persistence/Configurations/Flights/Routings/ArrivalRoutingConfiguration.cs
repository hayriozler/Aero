using AeroFlight.Domain.Ops.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Routings;

internal sealed class ArrivalRoutingConfiguration : IEntityTypeConfiguration<OpsArrivalRouting>
{
    public void Configure(EntityTypeBuilder<OpsArrivalRouting> builder)
    {
        builder.ToTable("ArrivalRouting", "ops");
        builder.Property(r => r.Id).UseSequence("FlightResourceSequence");
    }
}
