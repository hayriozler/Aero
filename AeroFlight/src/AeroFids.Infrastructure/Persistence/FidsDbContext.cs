using AeroFids.Infrastructure.Boards;
using AeroFids.Infrastructure.Flights;
using AeroFids.Infrastructure.Identity;
using AeroFids.Infrastructure.Settings;
using AeroFids.Infrastructure.Signage;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Infrastructure.Persistence;

public sealed class FidsDbContext(DbContextOptions<FidsDbContext> options) : DbContext(options)
{
    public DbSet<FidsUser> FidsUserSet => Set<FidsUser>();
    public DbSet<DisplayClient> DisplayClientSet => Set<DisplayClient>();
    public DbSet<FidsSetting> FidsSettingSet => Set<FidsSetting>();
    public DbSet<FieldMapping> FieldMappingSet => Set<FieldMapping>();
    public DbSet<StatusMapping> StatusMappingSet => Set<StatusMapping>();
    public DbSet<SignageImage> SignageImageSet => Set<SignageImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FidsUser>(entity =>
        {
            entity.ToTable("FidsUser");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(30);
        });

        modelBuilder.Entity<DisplayClient>(entity =>
        {
            entity.ToTable("DisplayClient");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(d => d.Name).IsUnique();
            entity.Property(d => d.Type).IsRequired().HasMaxLength(20);
            entity.Property(d => d.ResourceCode).HasMaxLength(20);
            entity.Property(d => d.Location).HasMaxLength(200);
            entity.Property(d => d.BankName).HasMaxLength(100);
            entity.Property(d => d.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<FieldMapping>(entity =>
        {
            entity.ToTable("FieldMapping");
            entity.HasKey(f => f.Id);
            entity.Property(f => f.TargetField).IsRequired().HasMaxLength(30);
            entity.Property(f => f.FlightType).HasMaxLength(20);
            entity.Property(f => f.SourcePath).IsRequired().HasMaxLength(200);
            entity.Property(f => f.RuleOperator).HasMaxLength(20);
            entity.Property(f => f.RuleValue).HasMaxLength(200);
            // Not DB-unique — Postgres treats NULL FlightType as distinct, so "one default row per
            // target" is enforced in FieldMappingService instead. This index is just for lookups.
            entity.HasIndex(f => new { f.TargetField, f.FlightType });
        });

        modelBuilder.Entity<StatusMapping>(entity =>
        {
            entity.ToTable("StatusMapping");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Dimension).IsRequired().HasMaxLength(20);
            entity.Property(s => s.FlightType).HasMaxLength(20);
            entity.Property(s => s.ExternalValue).IsRequired().HasMaxLength(50);
            entity.Property(s => s.InternalValue).IsRequired().HasMaxLength(20);
            // Not DB-unique — Postgres treats NULL FlightType as distinct, so "one row per
            // (Dimension, FlightType, ExternalValue)" is enforced in StatusMappingService instead.
            entity.HasIndex(s => new { s.Dimension, s.FlightType, s.ExternalValue });
        });

        modelBuilder.Entity<SignageImage>(entity =>
        {
            entity.ToTable("SignageImage");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            entity.Property(s => s.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(s => s.ImageData).IsRequired();
        });

        modelBuilder.Entity<FidsSetting>(entity =>
        {
            entity.ToTable("FidsSetting");
            entity.HasKey(s => s.Key);
            entity.Property(s => s.Key).HasMaxLength(100);
            entity.Property(s => s.Value).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
