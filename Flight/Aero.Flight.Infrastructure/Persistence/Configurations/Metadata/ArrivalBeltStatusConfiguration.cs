using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class ArrivalBeltStatusConfiguration : IEntityTypeConfiguration<ArrivalBeltStatus>
{
    public void Configure(EntityTypeBuilder<ArrivalBeltStatus> builder)
    {
        builder.ToTable("ArrivalBeltStatus", "metadata");
        builder.HasKey(b => b.Code);
        builder.Property(b => b.Code).IsRequired().HasMaxLength(10).HasColumnType("citext");
        builder.Property(b => b.Description).HasMaxLength(200);
        builder.Property(b => b.IsDefault).HasDefaultValue(false);
        builder.Property(b => b.IsOpenValue).HasDefaultValue(false);
        builder.Property(b => b.IsCloseValue).HasDefaultValue(false);
    }
}
