using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wanderlust.Core.Domain;
using Wanderlust.Application.Common.Interfaces;
using Wanderlust.Infrastructure.Configurations;

namespace Wanderlust.Infrastructure.Persistence;

public class CitiesRepository : ICitiesRepository
{
    private readonly IMongoCollection<City> _collection;

    public CitiesRepository(
        IMongoClient client,
        IOptions<LocationsDatabaseSettings> options)
    {
        _collection = client.GetDatabase(options.Value.DatabaseName)
            .GetCollection<City>(options.Value.CitiesCollectionName);
    }

    public async Task<City?> GetCityByCodeAsync(
        string cityCode, CancellationToken cancellationToken)
    {
        return await _collection
            .Find(c => c.Code == cityCode)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<City>> GetCitiesByCodesAsync(
        IEnumerable<string> cityCodes, CancellationToken cancellationToken)
    {
        var filter = Builders<City>.Filter.In(c => c.Code, cityCodes);

        return await _collection
            .Find(filter)
            .ToListAsync(cancellationToken);
    }
}
