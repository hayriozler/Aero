using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class ArrivalCodeShareConfiguration : IEntityTypeConfiguration<OpsArrivalCodeShare>
{
    public void Configure(EntityTypeBuilder<OpsArrivalCodeShare> builder)
    {
        builder.ToTable("ArrivalCodeShare", "ops");
        builder.Property(c => c.Id).UseSequence("FlightResourceSequence");
    }
}
