using Aero.Shared.Domain;
using Aero.Shared.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aero.Shared.Infrastructure.Persistence;

public abstract class DataStoreContext(DbContextOptions<DataStoreContext> options)
    : DbContext(options), IDataStoreContext
{
    public DbSet<User> UserSet => Set<User>();

    public DbSet<Role> RoleSet => Set<Role>();

    public DbSet<UserRole> UserRoleSet => Set<UserRole>();

    public DbSet<Permission> PermissionSet => Set<Permission>();

    public DbSet<RolePermission> RolePermissionSet => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokenSet => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        entry.Entity.CreatedBy = "SYSTEM";
                    if (string.IsNullOrEmpty(entry.Entity.UpdatedBy))
                        entry.Entity.UpdatedBy = "SYSTEM";
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    if (string.IsNullOrEmpty(entry.Entity.UpdatedBy))
                        entry.Entity.UpdatedBy = "SYSTEM";
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class => Set<TEntity>();
    public async ValueTask<TEntity?> FindAsync<TEntity>(string id, CancellationToken ct) where TEntity : class
        => await Set<TEntity>().FindAsync([id], ct);
    public void AddEntity<TEntity>(TEntity entity)
        where TEntity : class => Set<TEntity>().Add(entity);
    public void RemoveEntity<TEntity>(TEntity entity)
        where TEntity : class => Set<TEntity>().Remove(entity);
}