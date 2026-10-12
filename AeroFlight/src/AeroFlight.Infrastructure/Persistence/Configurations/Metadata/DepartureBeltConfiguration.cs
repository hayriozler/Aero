using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class DepartureBeltConfiguration : IEntityTypeConfiguration<DepartureBelt>
{
    public void Configure(EntityTypeBuilder<DepartureBelt> builder)
    {
        builder.ToTable("DepartureBelt", "metadata");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(b => b.Description).HasMaxLength(200);
        builder.Property(b => b.TerminalCode).HasMaxLength(10);
        builder.Property(b => b.Capacity).HasDefaultValue(1);

        builder.HasOne<Terminal>().WithMany().HasForeignKey(b => b.TerminalCode);

        AuditConfiguration.Apply<DepartureBelt, string>(builder);
        builder.HasAuditType<DepartureBelt, DepartureBeltAudit>();
    }
}
