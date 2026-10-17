using Aero.Scanner.Domain.Aggregates.Permissions;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "references");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Group)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
        builder.Property(p => p.UpdatedBy).HasMaxLength(20).IsRequired();

        builder.HasAuditType<Permission, PermissionLog>();
        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasIndex(p => p.Group);
    }
}
