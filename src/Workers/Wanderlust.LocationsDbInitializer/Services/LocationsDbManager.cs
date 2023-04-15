using Wanderlust.Core.Domain.Abstractions;
using Wanderlust.LocationsDbInitializer.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class LocationsDbManager : ILocationsDbManager
{
    private readonly ILocationRepositoryFactory _repositoryFactory;

    public LocationsDbManager(ILocationRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task PushDataAsync<TEntity>(
        IEnumerable<TEntity> locations,
        CancellationToken cancellationToken) where TEntity : ILocation
    {
        var repo = _repositoryFactory.GetRepository<TEntity>();

        await repo.CreateStorageAsync(cancellationToken);
        await repo.SeedDataAsync(locations, cancellationToken);
        await repo.CreateIndexesAsync(cancellationToken);
    }
}
