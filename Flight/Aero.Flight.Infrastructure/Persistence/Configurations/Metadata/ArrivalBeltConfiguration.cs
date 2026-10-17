using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class ArrivalBeltConfiguration : IEntityTypeConfiguration<ArrivalBelt>
{
    public void Configure(EntityTypeBuilder<ArrivalBelt> builder)
    {
        builder.ToTable("ArrivalBelt", "metadata");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(b => b.Description).HasMaxLength(200);
        builder.Property(b => b.TerminalCode).HasMaxLength(10);
        builder.Property(b => b.Capacity).HasDefaultValue(1);

        builder.HasOne<Terminal>().WithMany().HasForeignKey(b => b.TerminalCode);

        AuditConfiguration.Apply<ArrivalBelt, string>(builder);
        builder.HasAuditType<ArrivalBelt, ArrivalBeltAudit>();
    }
}
