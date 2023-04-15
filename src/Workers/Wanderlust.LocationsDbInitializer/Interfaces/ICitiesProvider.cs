using Wanderlust.Core.Domain;

namespace Wanderlust.LocationsDbInitializer.Interfaces;

public interface ICitiesProvider
{
    Task<IEnumerable<City>> GetCitiesAsync(IEnumerable<Country> countries, CancellationToken cancellationToken);
}