using Aero.Flights.Domain.Audit.Ops.Resources;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class ParkPositionConfiguration : IEntityTypeConfiguration<OpsFlightParkPosition>
{
    public void Configure(EntityTypeBuilder<OpsFlightParkPosition> builder)
    {
        builder.ToTable("ParkPosition", "ops");
        builder.HasAuditType<OpsFlightParkPosition, OpsFlightParkPositionAudit>();
        builder.HasKey(s => s.Id);
        builder.HasOne(s => s.ArrivalFlight).WithMany(f => f.ParkPositions).HasForeignKey(s => s.ArrivalFlightId);
        builder.HasOne(s => s.DepartureFlight).WithMany(f => f.ParkPositions).HasForeignKey(s => s.DepartureFlightId);
        builder.Property(s => s.BeginPlan).IsRequired();
        builder.Property(s => s.EndPlan).IsRequired();
        builder.Property(s => s.BeginActual);
        builder.Property(s => s.EndActual);
        builder.Property(s => s.Id).UseSequence("FlightResourceSequence");
        builder.Property(s => s.Code).HasMaxLength(10);
        builder.Property(s => s.LastCode).HasMaxLength(10);
        builder.Property(s => s.TowCode).HasMaxLength(10);
        builder.Property(s => s.TowIndicator).HasMaxLength(5);
        builder.Property(s => s.HandlingAgentCode).HasMaxLength(10);
        builder.Property(s => s.PayerCompanyCode).HasMaxLength(10).HasColumnType("citext");
        builder.Property(s => s.FlightStatusCode).HasMaxLength(5);
        builder.HasIndex(s => new { s.Code, s.BeginPlan, s.EndPlan });
    }
}
