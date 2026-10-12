using AeroFlight.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Flights.Identity;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permission", "identity");
        builder.HasKey(p => p.Code);
        builder.Property(p => p.Code).HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(200);
    }
}

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission", "identity");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionCode });
        builder.Property(rp => rp.PermissionCode).HasMaxLength(50);

        builder.HasOne<Role>().WithMany().HasForeignKey(rp => rp.RoleId);
        builder.HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionCode);
    }
}
