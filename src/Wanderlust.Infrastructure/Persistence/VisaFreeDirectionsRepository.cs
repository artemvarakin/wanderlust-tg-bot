using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wanderlust.Core.Domain;
using Wanderlust.Application.Common.Interfaces;
using Wanderlust.Infrastructure.Configurations;

namespace Wanderlust.Infrastructure.Persistence;

public class VisaFreeDirectionsRepository : IVisaFreeDirectionsRepository
{
    private readonly IMongoCollection<VisaFreeDirection> _collection;

    public VisaFreeDirectionsRepository(
        IMongoClient client,
        IOptions<LocationsDatabaseSettings> options)
    {
        _collection = client.GetDatabase(options.Value.DatabaseName)
            .GetCollection<VisaFreeDirection>(options.Value.VisaFreeDirectionsCollectionName);
    }

    public async Task<IEnumerable<string>> GetDirectionCodesByRegionAsync(
        GeoRegion region,
        CancellationToken cancellationToken)
    {
        return await _collection
            .Find(d => d.Region == region)
            .Project(d => d.Country.Code)
            .ToListAsync(cancellationToken);
    }
}
