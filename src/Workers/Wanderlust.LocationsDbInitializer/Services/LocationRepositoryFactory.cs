using Microsoft.Extensions.DependencyInjection;
using Wanderlust.Core.Domain.Abstractions;
using Wanderlust.LocationsDbInitializer.Data.Repositories;
using Wanderlust.LocationsDbInitializer.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class LocationRepositoryFactory : ILocationRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public LocationRepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // haven't figured out how to solve this without using service locator
    public BaseLocationsRepository<TEntity> GetRepository<TEntity>() where TEntity : ILocation
    {
        return _serviceProvider.GetRequiredService<BaseLocationsRepository<TEntity>>();
    }
}
