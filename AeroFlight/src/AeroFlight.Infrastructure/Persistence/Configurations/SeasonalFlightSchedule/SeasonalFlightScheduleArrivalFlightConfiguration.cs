using AeroFlight.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class SeasonalFlightScheduleArrivalFlightConfiguration : IEntityTypeConfiguration<SeasonalFlightScheduleArrivalFlight>
{
    public void Configure(EntityTypeBuilder<SeasonalFlightScheduleArrivalFlight> builder)
    {
        builder.ToTable("SeasonalFlightScheduleArrivalFlight", "ops");

        builder.HasMany(f => f.Routings).WithOne(r => r.Flight).HasForeignKey(r => r.SeasonalFlightScheduleFlightId);

        builder.Property(f => f.Id).UseSequence("SeasonalFlightScheduleArrivalFlightSequence");
        builder.Property(f => f.OriginAirportCode).IsRequired().HasMaxLength(4);
    }
}
