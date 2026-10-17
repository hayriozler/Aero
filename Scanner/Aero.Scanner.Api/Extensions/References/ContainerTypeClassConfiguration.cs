using Aero.Scanner.Domain.Aggregates.Containers;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class ContainerTypeClassConfiguration : IEntityTypeConfiguration<ContainerTypeClass>
{
    public void Configure(EntityTypeBuilder<ContainerTypeClass> builder)
    {
        builder.ToTable("ContainerTypeClasses", "references");
        builder.HasKey(u => new { u.TypeCode, u.ClassCode });

        builder.Property(u => u.TypeCode)
            .HasColumnName("TypeCode")
            .HasMaxLength(1)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(u => u.ClassCode)
            .HasMaxLength(1)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(u => u.Description)
            .HasMaxLength(20);

        builder.HasAuditType<ContainerTypeClass, ContainerTypeClassLog>();

        builder.HasAuditDisplayName(p => p.TypeCode, "Type Code");
        builder.HasAuditDisplayName(p => p.ClassCode, "Class Code");
        builder.HasAuditDisplayName(p => p.Description, "Description");
    }
}
