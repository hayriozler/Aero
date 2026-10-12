using AeroFlight.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleArrivalFlightRoutingConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleArrivalFlightRouting>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleArrivalFlightRouting> builder)
    {
        builder.ToTable("SeasonalFlightScheduleArrivalFlightRouting", "ops");
        builder.Property(r => r.Id).UseSequence("SeasonalFlightScheduleArrivalFlightRoutingSequence");
    }
}
