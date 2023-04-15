using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Wanderlust.Core.Domain;

namespace Wanderlust.LocationsDbInitializer.Data.Repositories;

public class VisaFreeDirectionsRepository : BaseLocationsRepository<VisaFreeDirection>
{
    public VisaFreeDirectionsRepository(ILogger<VisaFreeDirection> logger, IMongoClient client)
        : base(logger, client, LocationsDatabase.VisaFreeDirectionsCollectionName)
    {
    }

    public override async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        var idxModel = new CreateIndexModel<VisaFreeDirection>(
        Builders<VisaFreeDirection>.IndexKeys.Ascending(d => d.Region));

        await Collection.Indexes.CreateOneAsync(idxModel, null, cancellationToken);

        Logger.LogInformation(
            "Created index for '{Field}' field in '{Name}' collection.",
            nameof(VisaFreeDirection.Region),
            Collection.CollectionNamespace.CollectionName);
    }
}
