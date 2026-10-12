using Domain.Aggregates.Flights;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public interface IDataContext
{

}
public abstract class DataStoreContext(DbContextOptions options)
    : DbContext(options), IDataContext
{
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
}
