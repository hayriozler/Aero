using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AirlineConfiguration : IEntityTypeConfiguration<Airline>
{
    public void Configure(EntityTypeBuilder<Airline> builder)
    {
        builder.ToTable("Airline", "metadata");
        builder.HasKey(a => a.Id);        
        builder.Property(a => a.Id).HasMaxLength(3).HasColumnType("citext").HasColumnName("Code");
        builder.Property(a => a.IataCode).IsRequired().HasMaxLength(3).HasColumnType("citext");
        builder.Property(a => a.IcaoCode).HasMaxLength(4).HasColumnType("citext");
        builder.Property(a => a.FlightNumberCode).HasMaxLength(3).HasColumnType("citext");
        builder.Property(a => a.Callsign).HasMaxLength(50);
        builder.Property(a => a.Name).HasMaxLength(100);
        builder.Property(a => a.DisplayName).HasMaxLength(100);
        builder.Property(a => a.ParentAirlineCode).HasMaxLength(10).HasColumnType("citext");
        builder.Property(a => a.CountryCode).HasMaxLength(3).HasColumnType("citext");

        builder.HasOne<Airline>().WithMany().HasForeignKey(a => a.ParentAirlineCode);
        builder.HasOne<Country>().WithMany().HasForeignKey(a => a.CountryCode);

        AuditConfiguration.Apply<Airline, string>(builder);
        builder.HasAuditType<Airline, AirlineAudit>();
    }
}
