using Aero.Flights.Domain.Audit.Ops.Resources;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class DepartureCounterConfiguration : IEntityTypeConfiguration<OpsCounter>
{
    public void Configure(EntityTypeBuilder<OpsCounter> builder)
    {
        builder.ToTable("DepartureCounter", "ops");
        builder.Property(c => c.Id).UseSequence("FlightResourceSequence");
        builder.Property(c => c.CheckinClassCode).HasMaxLength(10);
        builder.HasIndex(c => new { c.Code, c.BeginPlan, c.EndPlan });
        builder.HasOne<CounterStatus>().WithMany().HasForeignKey(c => c.StatusCode);
        builder.HasAuditType<OpsCounter, OpsCounterAudit>();
    }
}
