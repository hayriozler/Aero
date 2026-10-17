using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class DelayReasonConfiguration : IEntityTypeConfiguration<DelayReason>
{
    public void Configure(EntityTypeBuilder<DelayReason> builder)
    {
        builder.ToTable("DelayReason", "metadata");
        builder.HasKey(d => d.Id);
         builder.Property(d => d.Id).HasMaxLength(4).HasColumnType("citext").HasColumnName("Code");
        builder.Property(d => d.AlphabeticCode).HasMaxLength(5).HasColumnType("citext");
        builder.Property(d => d.Description).HasMaxLength(200);

        AuditConfiguration.Apply<DelayReason, string>(builder);
        builder.HasAuditType<DelayReason, DelayReasonAudit>();


    }
}
