using Aero.Scanner.Domain.Aggregates.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class ContainerTypeConfiguration : IEntityTypeConfiguration<ContainerType>
{
    public void Configure(EntityTypeBuilder<ContainerType> builder)
    {
        builder.ToTable("ContainerTypes", "references");

        builder.HasKey(u => u.Code);

        builder.Property(u => u.Code)
            .HasMaxLength(1)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(u => u.IsAllDestination)
            .IsRequired();

        builder.Property(u => u.IsTransfer)
            .IsRequired();

        builder.Property(u => u.Description)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasAuditType<ContainerType, ContainerTypeLog>();

        builder.HasAuditDisplayName(p => p.Code, "Type Code");
        builder.HasAuditDisplayName(p => p.IsAllDestination, "All Dest");
        builder.HasAuditDisplayName(p => p.IsTransfer, "Trans?");
        builder.HasAuditDisplayName(p => p.Description, "Description");

    }
}
