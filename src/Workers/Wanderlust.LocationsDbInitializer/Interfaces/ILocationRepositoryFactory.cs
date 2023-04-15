using Wanderlust.Core.Domain.Abstractions;
using Wanderlust.LocationsDbInitializer.Data.Repositories;

namespace Wanderlust.LocationsDbInitializer.Interfaces;

public interface ILocationRepositoryFactory
{
    BaseLocationsRepository<TEntity> GetRepository<TEntity>() where TEntity : ILocation;
}