using AeroFlight.Domain.Audit.Ops.Resources;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class ArrivalGateConfiguration : IEntityTypeConfiguration<OpsArrivalGate>
{
    public void Configure(EntityTypeBuilder<OpsArrivalGate> builder)
    {
        builder.ToTable("ArrivalGate", "ops");
        builder.Property(g => g.Id).UseSequence("FlightResourceSequence");
        builder.HasIndex(g => new { g.Code, g.BeginPlan, g.EndPlan });
        builder.HasOne<GateStatus>().WithMany().HasForeignKey(g => g.StatusCode);
        builder.HasAuditType<OpsArrivalGate, OpsArrivalGateAudit>();
    }
}
