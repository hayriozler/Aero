using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class RemarkConfiguration : IEntityTypeConfiguration<Remark>
{
    public void Configure(EntityTypeBuilder<Remark> builder)
    {
        builder.ToTable("Remark", "metadata");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).IsRequired().HasMaxLength(20).HasColumnType("citext").HasColumnName("Code");
        builder.Property(r => r.Description).HasMaxLength(50);

        AuditConfiguration.Apply<Remark, string>(builder);
        builder.HasAuditType<Remark, RemarkAudit>();

    }
}
