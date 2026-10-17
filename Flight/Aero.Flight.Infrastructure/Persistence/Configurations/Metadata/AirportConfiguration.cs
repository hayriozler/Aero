using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.ToTable("Airport", "metadata");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).IsRequired().HasMaxLength(3).HasColumnType("citext").HasColumnName("Code");
        builder.Property(a => a.IataCode).HasMaxLength(3).HasColumnType("citext");
        builder.Property(a => a.IcaoCode).HasMaxLength(4).HasColumnType("citext");
        builder.Property(a => a.Name).HasMaxLength(100);
        builder.Property(a => a.DisplayName).HasMaxLength(100);
        builder.Property(a => a.CountryCode).HasMaxLength(3).HasColumnType("citext");
        builder.Property(a => a.TimezoneCode).HasMaxLength(50).HasColumnType("citext");

        builder.HasOne<Country>().WithMany().HasForeignKey(a => a.CountryCode);
        builder.HasOne<Timezone>().WithMany().HasForeignKey(a => a.TimezoneCode);

        AuditConfiguration.Apply<Airport, string>(builder);
        builder.HasAuditType<Airport, AirportAudit>();

    }
}
