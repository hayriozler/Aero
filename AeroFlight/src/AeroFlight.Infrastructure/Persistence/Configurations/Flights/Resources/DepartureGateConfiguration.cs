using AeroFlight.Domain.Audit.Ops.Resources;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class DepartureGateConfiguration : IEntityTypeConfiguration<OpsDepartureGate>
{
    public void Configure(EntityTypeBuilder<OpsDepartureGate> builder)
    {
        builder.ToTable("DepartureGate", "ops");
        builder.Property(g => g.Id).UseSequence("FlightResourceSequence");
        builder.Property(g => g.BoardingPlan);
        builder.Property(g => g.BoardingActual);
        builder.HasIndex(g => new { g.Code, g.BeginPlan, g.EndPlan });
        builder.HasOne<GateStatus>().WithMany().HasForeignKey(g => g.StatusCode);
        builder.HasAuditType<OpsDepartureGate, OpsDepartureGateAudit>();
    }
}
