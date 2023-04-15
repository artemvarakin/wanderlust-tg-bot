using Wanderlust.Core.Domain;

namespace Wanderlust.LocationsDbInitializer.Interfaces;

public interface ICountriesProvider
{
    Task<IEnumerable<Country>> GetCountriesAsync(CancellationToken cancellationToken);
}