using Aero.Flights.Domain.Ops.Delays;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Delays;

internal sealed class ArrivalDelayConfiguration : IEntityTypeConfiguration<OpsArrivalDelay>
{
    public void Configure(EntityTypeBuilder<OpsArrivalDelay> builder)
    {
        builder.ToTable("ArrivalDelay", "ops");
        builder.Property(d => d.Id).UseSequence("FlightResourceSequence");
    }
}
