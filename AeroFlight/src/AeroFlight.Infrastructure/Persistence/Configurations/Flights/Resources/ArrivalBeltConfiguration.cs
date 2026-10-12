using AeroFlight.Domain.Audit.Ops.Resources;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class ArrivalBeltConfiguration : IEntityTypeConfiguration<OpsArrivalBelt>
{
    public void Configure(EntityTypeBuilder<OpsArrivalBelt> builder)
    {
        builder.ToTable("ArrivalBelt", "ops");
        builder.Property(b => b.Id).UseSequence("FlightResourceSequence");
        builder.Property(b => b.LastBag);
        builder.HasIndex(b => new { b.Code, b.BeginPlan, b.EndPlan });
        builder.HasOne<ArrivalBeltStatus>().WithMany().HasForeignKey(b => b.StatusCode);
        builder.HasAuditType<OpsArrivalBelt, OpsArrivalBeltAudit>();
    }
}
