using Aero.Scanner.Domain.Aggregates.Flights;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Persistence.Configurations.Flights;

internal sealed class ArrivalFlightConfiguration : IEntityTypeConfiguration<ArrivalFlight>
{
    public void Configure(EntityTypeBuilder<ArrivalFlight> builder)
    {
        builder.ToTable("ArrivalFlights", "flights");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).UseIdentityColumn();

        builder.Property(f => f.RemoteSystemId).HasMaxLength(25).HasColumnType("citext").IsRequired();
        builder.Property(f => f.AirlineCode).HasMaxLength(5).HasColumnType("citext").IsRequired();
        builder.Property(f => f.FlightNumber).HasMaxLength(7).HasColumnType("citext").IsRequired();
        builder.Property(f => f.FlightIataDate).HasMaxLength(5).HasColumnType("citext").IsRequired();
        builder.Property(f => f.ScheduledDateTime).IsRequired();
        builder.Property(f => f.EstimatedDateTime);
        builder.Property(f => f.OriginAirport).HasMaxLength(3).IsRequired();
        builder.Property(f => f.PreviousAirport).HasMaxLength(3).IsRequired();
        builder.Property(f => f.Carousel).HasMaxLength(10);

        builder.Property(f => f.FlightStatus).HasConversion<short>().IsRequired();


        builder.HasOne(f => f.HandlingCompany)
            .WithMany()
            .HasForeignKey(f => f.HandlingCompanyCode)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(f => f.ReconciliationRecords)
           .WithOne()
           .HasForeignKey(l => l.FlightId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasAuditType<ArrivalFlight, ArrivalFlightLog>();

        builder.HasIndex(f => f.RemoteSystemId).IsUnique();
        builder.HasIndex(f => new { f.ScheduledDateTime, f.HandlingCompanyCode, f.FlightStatus });
        builder.HasIndex(f => new { f.AirlineCode, f.FlightNumber, f.FlightIataDate });
        builder.HasIndex(f => f.HandlingCompanyCode);

        builder.Ignore(p => p.Destination);
        builder.IgnoreAudit(p => p.Destination);

        builder.Navigation(n => n.ReconciliationRecords).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

