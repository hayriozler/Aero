using Aero.Scanner.Domain.Aggregates.Roles;
using Aero.Scanner.Domain.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "references");
        builder.HasKey(r => r.Name);

        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(r => r.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();

        builder.HasAuditType<Role, RoleLog>();
        builder.HasIndex(r => r.Name).IsUnique();

        builder.HasAuditDisplayName(e => e.Name, "System Name");
        builder.HasAuditDisplayName(e => e.DisplayName, "Display Name");
    }
}
