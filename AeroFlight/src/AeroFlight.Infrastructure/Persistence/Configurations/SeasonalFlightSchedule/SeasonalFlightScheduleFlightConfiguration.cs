using AeroFlight.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleFlightConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleFlight>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleFlight> builder)
    {
        builder.HasKey(f => f.Id);
        AuditConfiguration.Apply<SeasonalFlightScheduleFlight, int>(builder);

        builder.HasOne(f => f.Season).WithMany().HasForeignKey(f => f.ScheduleSeasonCode);

        builder.Property(f => f.ScheduleSeasonCode).IsRequired().HasMaxLength(15);
        builder.Property(f => f.AirlineCode).IsRequired().HasMaxLength(3);
        builder.Property(f => f.FlightNumber).IsRequired().HasMaxLength(10);
        builder.Property(f => f.CallSign).HasMaxLength(20);
        builder.Property(f => f.ScheduleFlightStatusCode).HasMaxLength(10);
        builder.Property(f => f.CountryType).HasMaxLength(10);
        builder.Property(f => f.AircraftTypeCode).HasMaxLength(10);
        builder.Property(f => f.OpsDays).HasMaxLength(20);
        builder.Property(f => f.DirectionCode).HasMaxLength(3);
        builder.Property(f => f.Comment).HasMaxLength(500);
    }
}
