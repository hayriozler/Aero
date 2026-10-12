using AeroFlight.Domain.Identity;
using AeroFlight.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Infrastructure.Tenancy;

public sealed class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> TenantSet => Set<Tenant>();
    public DbSet<ClientApp> ClientAppSet => Set<ClientApp>();
    public DbSet<TenantInterface> TenantInterfaceSet => Set<TenantInterface>();
    public DbSet<TenantInterfaceSetting> TenantInterfaceSettingSet => Set<TenantInterfaceSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenant");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasMaxLength(50).HasColumnType("citext");
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.ConnectionStringProtected).HasMaxLength(400).IsRequired();
            entity.Property(t => t.HomeAirportCode).HasMaxLength(4).IsRequired();
            entity.HasIndex(t => t.IsActive);
        });
        modelBuilder.Entity<ClientApp>(entity =>
        {
            entity.ToTable("ClientApp");
            // Name alone isn't unique across tenants — every tenant gets its own "AeroFids" row.
            entity.HasKey(t => new { t.Name, t.TenantId });
            entity.Property(t => t.Name).HasMaxLength(35);
            entity.Property(t => t.TenantId).HasMaxLength(50);
            entity.Property(t => t.Secret).HasMaxLength(128);
            entity.Property(t => t.BeginDateTime);
            entity.Property(t => t.EndDateTime);
            entity.HasOne<Tenant>().WithMany().HasForeignKey(t => t.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<TenantInterface>(entity =>
        {
            entity.ToTable("TenantInterface");
            entity.HasKey(t => new { t.TenantId, t.InterfaceCode });
            entity.Property(t => t.TenantId).HasMaxLength(50);
            entity.Property(t => t.InterfaceCode).HasMaxLength(20);
            entity.HasOne<Tenant>().WithMany().HasForeignKey(t => t.TenantId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<TenantInterfaceSetting>(entity =>
        {
            entity.ToTable("TenantInterfaceSetting");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.TenantId).HasMaxLength(50).IsRequired();
            entity.Property(s => s.Key).HasMaxLength(100).IsRequired();
            entity.Property(s => s.Value).IsRequired();
            entity.HasIndex(s => new { s.TenantId, s.Key }).IsUnique();
            entity.HasOne<Tenant>().WithMany().HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
