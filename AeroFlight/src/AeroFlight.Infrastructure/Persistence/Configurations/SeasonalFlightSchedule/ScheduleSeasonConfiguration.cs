using AeroFlight.Domain.SeasonalFlightSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.SeasonalFlightSchedule;

internal sealed class ScheduleSeasonConfiguration : IEntityTypeConfiguration<ScheduleSeason>
{
    public void Configure(EntityTypeBuilder<ScheduleSeason> builder)
    {
        builder.ToTable("ScheduleSeason", "ops");
        builder.HasKey(s => s.Id);
        AuditConfiguration.Apply<ScheduleSeason, string>(builder);

        builder.Property(s => s.Id).IsRequired().HasMaxLength(15);
        builder.Property(s => s.Description).HasMaxLength(200);
    }
}
