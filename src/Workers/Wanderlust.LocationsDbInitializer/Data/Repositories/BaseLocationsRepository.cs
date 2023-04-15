using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Wanderlust.Core.Domain.Abstractions;

namespace Wanderlust.LocationsDbInitializer.Data.Repositories;

public abstract class BaseLocationsRepository<TEntity> where TEntity : ILocation
{
    protected readonly ILogger<TEntity> Logger;
    protected readonly IMongoDatabase Database;
    protected readonly IMongoCollection<TEntity> Collection;

    protected BaseLocationsRepository(ILogger<TEntity> logger, IMongoClient client, string collectionName)
    {
        Logger = logger;
        Database = client.GetDatabase(LocationsDatabase.DatabaseName);
        Collection = Database.GetCollection<TEntity>(collectionName);
    }

    public virtual async Task CreateStorageAsync(CancellationToken cancellationToken)
    {
        await DropCollectionIfExistsAsync(cancellationToken);

        var defaultCollation = new Collation(locale: "en", strength: CollationStrength.Primary);
        var options = new CreateCollectionOptions
        {
            Collation = defaultCollation
        };

        await Database.CreateCollectionAsync(
            Collection.CollectionNamespace.CollectionName,
            options,
            cancellationToken);

        Logger.LogInformation(
            "Created '{Name}' collection with default collation (locale: {Locale}, strength: {Strength}).",
            Collection.CollectionNamespace.CollectionName,
            defaultCollation.Locale,
            defaultCollation.Strength!.Value);
    }

    public virtual async Task SeedDataAsync(IEnumerable<TEntity> data, CancellationToken cancellationToken)
    {
        await Collection.InsertManyAsync(data, null, cancellationToken);

        Logger.LogInformation(
            "{Count} entities of type {Type} added to '{Name}' collection.",
            data.Count(),
            typeof(TEntity).Name,
            Collection.CollectionNamespace.CollectionName);
    }

    public abstract Task CreateIndexesAsync(CancellationToken cancellationToken);

    private async Task DropCollectionIfExistsAsync(CancellationToken cancellationToken)
    {
        var collectionNames = await Database
            .ListCollectionNames(null, cancellationToken)
            .ToListAsync(cancellationToken);

        if (collectionNames.Contains(Collection.CollectionNamespace.CollectionName))
        {
            Logger.LogWarning(
                "Collection '{Name}' already exists. Will be dropped.",
                Collection.CollectionNamespace.CollectionName);

            await Database.DropCollectionAsync(
                Collection.CollectionNamespace.CollectionName,
                cancellationToken);

            Logger.LogInformation(
                "Dropped collection '{Name}'.",
                Collection.CollectionNamespace.CollectionName);
        }
    }
}