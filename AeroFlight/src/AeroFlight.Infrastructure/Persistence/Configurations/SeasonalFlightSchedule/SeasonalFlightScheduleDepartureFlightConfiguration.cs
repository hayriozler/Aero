using AeroFlight.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleDepartureFlightConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleDepartureFlight>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleDepartureFlight> builder)
    {
        builder.ToTable("SeasonalFlightScheduleDepartureFlight", "ops");

        builder.HasMany(f => f.Routings).WithOne(r => r.Flight).HasForeignKey(r => r.SeasonalFlightScheduleFlightId);

        builder.Property(f => f.Id).UseSequence("SeasonalFlightScheduleDepartureFlightSequence");
        builder.Property(f => f.DestinationAirportCode).IsRequired().HasMaxLength(4);
    }
}
