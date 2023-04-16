using Wanderlust.TravelpayoutsClients.Models;

namespace Wanderlust.TravelpayoutsClients.Interfaces;

public interface IDataApiClient
{
    Task<IEnumerable<CountryInfo>> GetCountriesInfoAsync(CancellationToken cancellationToken);
    Task<IEnumerable<CityInfo>> GetCitiesInfoAsync(CancellationToken cancellationToken);
}