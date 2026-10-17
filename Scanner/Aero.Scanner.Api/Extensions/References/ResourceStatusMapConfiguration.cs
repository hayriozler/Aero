using Aero.Scanner.Domain.Aggregates.Mappings;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class ResourceStatusMapConfiguration : IEntityTypeConfiguration<ResourceStatusMap>
{
    public void Configure(EntityTypeBuilder<ResourceStatusMap> builder)
    {
        builder.ToTable("ResourceMaps", "references");

        builder.HasKey(u => new { u.SourceResourceName, u.SourceResourceStatus });

        builder.Property(u => u.SourceResourceName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.SourceResourceStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.TargetResourceStatus)
            .HasMaxLength(1)
            .IsRequired();

        builder.HasAuditType<ResourceStatusMap, ResourceStatusMapLog>();
        builder.HasAuditDisplayName(e => e.SourceResourceName, "Source Resource");
        builder.HasAuditDisplayName(e => e.SourceResourceStatus, "Source Status");
        builder.HasAuditDisplayName(e => e.TargetResourceStatus, "Target Status");
    }
}
