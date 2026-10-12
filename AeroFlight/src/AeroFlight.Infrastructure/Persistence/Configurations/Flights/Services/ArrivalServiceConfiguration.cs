using AeroFlight.Domain.Ops.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Services;

internal sealed class ArrivalServiceConfiguration : IEntityTypeConfiguration<OpsArrivalService>
{
    public void Configure(EntityTypeBuilder<OpsArrivalService> builder)
    {
        builder.ToTable("ArrivalService", "ops");
        builder.Property(s => s.Id).UseSequence("FlightResourceSequence");
    }
}
