using Aero.Flights.Domain.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Scheduling;

internal sealed class JobScheduleConfiguration : IEntityTypeConfiguration<JobSchedule>
{
    public void Configure(EntityTypeBuilder<JobSchedule> builder)
    {
        builder.ToTable("JobSchedule", "ops");
        builder.HasKey(j => j.Id);
        AuditConfiguration.Apply<JobSchedule, string>(builder);

        builder.Property(j => j.Id).HasMaxLength(100);
        builder.Property(j => j.JobName).IsRequired().HasMaxLength(100);
        builder.Property(j => j.CronExpression).HasMaxLength(100);
    }
}
