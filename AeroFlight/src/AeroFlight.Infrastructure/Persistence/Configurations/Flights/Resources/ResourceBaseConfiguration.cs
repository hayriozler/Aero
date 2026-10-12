using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class ResourceBaseConfiguration : IEntityTypeConfiguration<ResourceBase>
{
    public void Configure(EntityTypeBuilder<ResourceBase> builder)
    {
        builder.HasKey(r => r.Id);

        // FlightId is a discovered FK property (via the owning flight's collection navigation), so
        // by EF Core convention it gets appended after every plain scalar column regardless of its
        // declaration order in the C# class — pin the physical column order explicitly instead.
        builder.Property(r => r.Id).HasColumnOrder(0);
        builder.Property(r => r.FlightId).IsRequired().HasColumnOrder(1);
        builder.Property(r => r.Code).HasMaxLength(20).HasColumnOrder(2);
        builder.Property(r => r.StatusCode).HasMaxLength(5).HasColumnOrder(3);
        builder.Property(r => r.BeginPlan).HasColumnOrder(4);
        builder.Property(r => r.EndPlan).HasColumnOrder(5);
        builder.Property(r => r.BeginActual).HasColumnOrder(6);
        builder.Property(r => r.EndActual).HasColumnOrder(7);
        builder.Property(r => r.LastCode).HasMaxLength(20);
        builder.Property(r => r.DisplayCode).HasMaxLength(10);
        builder.Property(r => r.PayerCompanyCode).HasMaxLength(10).HasColumnType("citext");
        builder.Property(r => r.IsComplimentary).HasColumnOrder(8).HasDefaultValue(false);
    }
}
