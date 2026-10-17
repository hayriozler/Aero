using Aero.Flights.Domain.Ops.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Services;

internal sealed class ArrivalServiceConfiguration : IEntityTypeConfiguration<OpsArrivalService>
{
    public void Configure(EntityTypeBuilder<OpsArrivalService> builder)
    {
        builder.ToTable("ArrivalService", "ops");
        builder.Property(s => s.Id).UseSequence("FlightResourceSequence");
    }
}
