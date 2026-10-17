using Aero.Flights.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleArrivalFlightRoutingConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleArrivalFlightRouting>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleArrivalFlightRouting> builder)
    {
        builder.ToTable("SeasonalFlightScheduleArrivalFlightRouting", "ops");
        builder.Property(r => r.Id).UseSequence("SeasonalFlightScheduleArrivalFlightRoutingSequence");
    }
}
