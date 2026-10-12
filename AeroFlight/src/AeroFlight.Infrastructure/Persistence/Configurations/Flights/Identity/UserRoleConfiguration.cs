using AeroFlight.Domain.Audit.Identity;
using AeroFlight.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Identity;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole", "identity");
        builder.HasKey(u => u.Id);
        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.RoleId).IsRequired();
        builder.HasIndex(u => new { u.UserId, u.RoleId }).IsUnique();

        builder.HasAuditType<UserRole, UserRoleAudit>();
        AuditConfiguration.Apply<UserRole, long>(builder);
    }
}
