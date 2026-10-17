using Aero.Flights.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleDepartureFlightRoutingConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleDepartureFlightRouting>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleDepartureFlightRouting> builder)
    {
        builder.ToTable("SeasonalFlightScheduleDepartureFlightRouting", "ops");
        builder.Property(r => r.Id).UseSequence("SeasonalFlightScheduleDepartureFlightRoutingSequence");
    }
}
