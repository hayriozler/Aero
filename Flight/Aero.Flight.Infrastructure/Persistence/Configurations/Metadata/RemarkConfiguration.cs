using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

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
