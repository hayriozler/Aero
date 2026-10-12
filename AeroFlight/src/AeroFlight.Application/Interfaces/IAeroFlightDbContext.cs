using AeroFlight.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Interfaces;

public interface IAeroFlightDbContext
{
    DbSet<User> UserSet { get; }
    DbSet<Role> RoleSet { get; }
    DbSet<UserRole> UserRoleSet { get; }

    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
    ValueTask<TEntity?> FindAsync<TEntity>(string id, CancellationToken ct) where TEntity : class;
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken ct);
}
