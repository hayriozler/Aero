using Aero.Flights.Domain.Common;
using Aero.Shared.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Infrastructure.Persistence;

public interface IDataStoreContext
{
    DbSet<AeroConfiguration> AeroConfigurationSet { get; }
    DbSet<User> UserSet { get; }
    DbSet<Role> RoleSet { get; }
    DbSet<UserRole> UserRoleSet { get; }
    DbSet<Permission> PermissionSet { get; }
    DbSet<RolePermission> RolePermissionSet { get; }
    DbSet<RefreshToken> RefreshTokenSet { get; }
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
    ValueTask<TEntity?> FindAsync<TEntity>(string id, CancellationToken ct) where TEntity : class;
    void AddEntity<TEntity>(TEntity entity) where TEntity : class;
    void RemoveEntity<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken ct);
}
