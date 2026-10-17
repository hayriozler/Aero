using System.Text.Json;
using Aero.Flights.Application.Interfaces;

namespace Aero.Flights.Application.Metadatas;

public interface IMetadataInboundApplier
{
    string Entity { get; }
    Task UpsertAsync(string code, JsonElement? data, CancellationToken ct);
    Task DeleteAsync(string code, CancellationToken ct);
}

public sealed class MetadataInboundApplier<TEntity, TRequest, TResponse>(
    IAeroFlightDbContext db, IMetadataMap<TEntity, TRequest, TResponse> map)
    : IMetadataInboundApplier
    where TEntity : class
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public string Entity => typeof(TEntity).Name;

    public async Task UpsertAsync(string code, JsonElement? data, CancellationToken ct)
    {
        if (data is not { } element || element.Deserialize<TRequest>(_jsonOptions) is not { } request)
            return;

        var entity = await db.FindAsync<TEntity>(code, ct);
        if (entity is null)
        {
            entity = map.Create(request);
            db.Add(entity);
        }

        map.Apply(entity, request);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(string code, CancellationToken ct)
    {
        var entity = await db.FindAsync<TEntity>(code, ct);
        if (entity is null)
            return;

        db.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
