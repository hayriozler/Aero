using Aero.Flights.Domain.Ops.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Services;

internal sealed class ServiceConfiguration : IEntityTypeConfiguration<OpsService>
{
    public void Configure(EntityTypeBuilder<OpsService> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s=>s.Id).UseSequence("FlightResourceSequence");
        builder.Property(s => s.FlightId).IsRequired();
        builder.Property(s => s.ServiceCode).IsRequired().HasMaxLength(10);
        builder.Property(s => s.AmountPlan).HasPrecision(18, 4);
        builder.Property(s => s.AmountActual).HasPrecision(18, 4);
        builder.Property(s => s.HandlingAgentCode).HasMaxLength(10);
        builder.Property(s => s.PayerCompanyCode).HasMaxLength(10).HasColumnType("citext");
        builder.Property(s => s.Comment).HasMaxLength(500);

    }
}
