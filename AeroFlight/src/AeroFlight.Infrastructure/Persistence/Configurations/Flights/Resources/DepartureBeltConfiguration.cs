using AeroFlight.Domain.Audit.Ops.Resources;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class DepartureBeltConfiguration : IEntityTypeConfiguration<OpsDepartureBelt>
{
    public void Configure(EntityTypeBuilder<OpsDepartureBelt> builder)
    {
        builder.ToTable("DepartureBelt", "ops");
        builder.Property(b => b.Id).UseSequence("FlightResourceSequence");
        builder.HasIndex(b => new { b.Code, b.BeginPlan, b.EndPlan });
        builder.HasOne<DepartureBeltStatus>().WithMany().HasForeignKey(b => b.StatusCode);
        builder.HasAuditType<OpsDepartureBelt, OpsDepartureBeltAudit>();
    }
}
