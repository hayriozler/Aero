using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Country", "metadata");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().HasColumnName("Code").HasMaxLength(5).HasColumnType("citext");
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CountryTypeCode).IsRequired().HasMaxLength(1).HasColumnType("citext");

        builder.HasOne(c => c.CountryType).WithMany().HasForeignKey(c => c.CountryTypeCode);
        builder.HasOne<Region>().WithMany().HasForeignKey(a => a.RegionCode);

        AuditConfiguration.Apply<Country, string>(builder);
        builder.HasAuditType<Country, CountryAudit>();

    }
}
