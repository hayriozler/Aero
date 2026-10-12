using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Audit.MetaData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class HandlingServiceConfiguration : IEntityTypeConfiguration<HandlingService>
{
    public void Configure(EntityTypeBuilder<HandlingService> builder)
    {
        builder.ToTable("HandlingService", "metadata");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).IsRequired().HasMaxLength(5).HasColumnType("citext").HasColumnName("Code");
        builder.Property(t => t.Unit).HasMaxLength(15);
        builder.Property(t => t.Description).HasMaxLength(200);

        AuditConfiguration.Apply<HandlingService, string>(builder);
        builder.HasAuditType<HandlingService, HandlingServiceAudit>();

    }
}
