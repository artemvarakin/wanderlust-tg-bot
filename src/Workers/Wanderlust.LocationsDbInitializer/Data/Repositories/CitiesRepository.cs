using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Wanderlust.Core.Domain;

namespace Wanderlust.LocationsDbInitializer.Data.Repositories;

public class CitiesRepository : BaseLocationsRepository<City>
{
    public CitiesRepository(ILogger<City> logger, IMongoClient client)
        : base(logger, client, LocationsDatabase.CitiesCollectionName)
    {
    }

    public override async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        var idxModels = new CreateIndexModel<City>[]
        {
            new CreateIndexModel<City>(Builders<City>.IndexKeys.Ascending(c => c.Code)),
            new CreateIndexModel<City>(Builders<City>.IndexKeys.Ascending(c => c.Country.Code))
        };

        await Collection.Indexes.CreateManyAsync(idxModels, null, cancellationToken);

        Logger.LogInformation(
            "Created indexes for '{CityCode}' and '{CountryCode}' fields in '{Name}' collection.",
            nameof(City.Code),
            string.Join('.', nameof(Country), nameof(Country.Code)),
            Collection.CollectionNamespace.CollectionName);
    }
}