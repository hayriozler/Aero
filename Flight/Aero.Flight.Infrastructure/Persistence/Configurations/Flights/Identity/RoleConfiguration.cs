using Aero.Flights.Domain.Audit.Identity;
using Aero.Flights.Domain.Identity;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Identity;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", "identity");
        builder.HasKey(r => r.Id);
        AuditConfiguration.Apply<Role, int>(builder);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Description).HasMaxLength(200);

        builder.HasMany(r => r.UserRoles).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId);

        builder.HasIndex(r => r.Name).IsUnique();

        builder.HasAuditType<Role, RoleAudit>();
    }
}
