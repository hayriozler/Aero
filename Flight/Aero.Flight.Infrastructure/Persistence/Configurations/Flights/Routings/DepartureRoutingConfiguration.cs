using Aero.Flights.Domain.Ops.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Routings;

internal sealed class DepartureRoutingConfiguration : IEntityTypeConfiguration<OpsDepartureRouting>
{
    public void Configure(EntityTypeBuilder<OpsDepartureRouting> builder)
    {
        builder.ToTable("DepartureRouting", "ops");
        builder.Property(r => r.Id).UseSequence("DepartureRoutingSequence");
    }
}
