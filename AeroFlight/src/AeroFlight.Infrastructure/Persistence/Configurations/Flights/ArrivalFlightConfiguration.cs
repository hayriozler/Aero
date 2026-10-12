using AeroFlight.Domain.Audit.Ops;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights;

internal sealed class ArrivalFlightConfiguration : IEntityTypeConfiguration<ArrivalFlight>
{
    public void Configure(EntityTypeBuilder<ArrivalFlight> builder)
    {
        builder.ToTable("ArrivalFlight", "ops");

        builder.HasMany(f => f.Passengers).WithOne().HasForeignKey(p => p.FlightId);
        builder.HasMany(f => f.OpsBelts).WithOne().HasForeignKey(b => b.FlightId);
        builder.HasMany(f => f.OpsGates).WithOne().HasForeignKey(g => g.FlightId);
        builder.HasMany(f => f.Routings).WithOne().HasForeignKey(r => r.FlightId);
        builder.HasMany(f => f.Services).WithOne().HasForeignKey(s => s.FlightId);
        builder.HasMany(f => f.CodeShares).WithOne().HasForeignKey(c => c.FlightId);
        builder.HasMany(f => f.Delays).WithOne().HasForeignKey(d => d.FlightId);

 

        builder.HasAuditType<ArrivalFlight, ArrivalFlightAudit>();

        builder.Property(f => f.Id).UseSequence("ArrivalFlightSequence");
        builder.Property(f => f.FlightDate).IsRequired().HasColumnType("date");
        builder.HasIndex(f => new { f.AirlineCode, f.FlightNumber, f.FlightDate }).IsUnique();
        builder.Property(f => f.DivertAirportCode).HasMaxLength(3);

        builder.HasOne<Airport>().WithMany().HasForeignKey(f => f.DivertAirportCode);

        builder.OwnsOne(f => f.LoadInfo, o =>
        {
            o.ToTable("ArrivalFlightLoadInfo", "ops");
            o.WithOwner().HasForeignKey("FlightId");
            o.Property(l => l.BaggageWeight).HasPrecision(18, 4);
            o.Property(l => l.BaggageTotal).HasPrecision(18, 4);
            o.Property(l => l.CargoWeight).HasPrecision(18, 4);
            o.Property(l => l.MailWeight).HasPrecision(18, 4);
            o.Property(l => l.TotalWeight).HasPrecision(18, 4);
            o.Property(l => l.EquipmentWeight).HasPrecision(18, 4);
            o.Property(l => l.CabinLuggage).HasPrecision(18, 4);
        });
    }
}
