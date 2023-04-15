using Wanderlust.Core.Domain.Abstractions;

namespace Wanderlust.LocationsDbInitializer.Interfaces;

public interface ILocationsDbManager
{
    Task PushDataAsync<TEntity>(IEnumerable<TEntity> locations, CancellationToken cancellationToken) where TEntity : ILocation;
}