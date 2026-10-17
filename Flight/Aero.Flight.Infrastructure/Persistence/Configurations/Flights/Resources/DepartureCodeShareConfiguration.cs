using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class DepartureCodeShareConfiguration : IEntityTypeConfiguration<OpsDepartureCodeShare>
{
    public void Configure(EntityTypeBuilder<OpsDepartureCodeShare> builder)
    {
        builder.ToTable("DepartureCodeShare", "ops");
        builder.Property(c => c.Id).UseSequence("FlightResourceSequence");
    }
}
