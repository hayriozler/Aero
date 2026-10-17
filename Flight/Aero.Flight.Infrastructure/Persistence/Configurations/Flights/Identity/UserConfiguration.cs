using Aero.Flights.Domain.Audit.Identity;
using Aero.Flights.Domain.Identity;
using Aero.Flights.Infrastructure;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Identity;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", "identity");
        builder.HasKey(u => u.Id);
    

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100).HasColumnType("citext");
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(u => u.RealName).HasMaxLength(200);

        builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);

        builder.HasIndex(u => u.Username).IsUnique();

        builder.HasAuditType<User, UserAudit>();
        AuditConfiguration.Apply<User, int>(builder);
        builder.IgnoreAudit(u => u.PasswordHash);
    }
}
