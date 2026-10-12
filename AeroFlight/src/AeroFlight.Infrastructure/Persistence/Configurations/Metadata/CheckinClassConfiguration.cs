using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CheckinClassConfiguration : IEntityTypeConfiguration<CheckinClass>
{
    public void Configure(EntityTypeBuilder<CheckinClass> builder)
    {
        builder.ToTable("CheckinClass", "metadata");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(c => c.Description).IsRequired().HasMaxLength(50);

        AuditConfiguration.Apply<CheckinClass, string>(builder);
        builder.HasAuditType<CheckinClass, CheckinClassAudit>();
    }
}
