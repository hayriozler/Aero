using Aero.Scanner.Domain.Aggregates.Users;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class UserConfiguration : IEntityTypeConfiguration<ScannerUser>
{
    public void Configure(EntityTypeBuilder<ScannerUser> builder)
    {
        builder.ToTable("Users", "references");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();
        builder.Property(u => u.LastLoginAt);

        builder.HasIndex(u => u.Username).IsUnique();

        builder.HasAuditType<ScannerUser, UserLog>();
        builder.HasAuditDisplayName(e => e.DisplayName, "Display Name");
        builder.HasAuditDisplayName(e => e.CompanyCode, "Company Code");
        builder.HasAuditDisplayName(e => e.LastLoginAt, "Last Login");

        builder.IgnoreAudit(e => e.PasswordHash);

        builder.HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
