using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleRoutingConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleRouting>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleRouting> builder)
    {
        builder.HasKey(r => r.Id);
        AuditConfiguration.Apply<SeasonalFlightScheduleRouting, int>(builder);

        builder.Property(r => r.SeasonalFlightScheduleFlightId).IsRequired();
        builder.Property(r => r.AirportCode).IsRequired().HasMaxLength(4);
        builder.Property(r => r.LegNumber).IsRequired();
        builder.Property(r => r.DirectionCode).IsRequired().HasMaxLength(3);
    }
}
