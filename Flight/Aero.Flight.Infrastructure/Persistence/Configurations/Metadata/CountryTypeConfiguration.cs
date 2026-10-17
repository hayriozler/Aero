using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CountryTypeConfiguration : IEntityTypeConfiguration<CountryType>
{
    public void Configure(EntityTypeBuilder<CountryType> builder)
    {
        builder.ToTable("CountryType", "metadata");
        builder.HasKey(c => c.Code);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(c => c.Description).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Severity).IsRequired();
    }
}
