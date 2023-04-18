using Wanderlust.Core.Domain.Abstractions;

namespace Wanderlust.TravelpayoutsClients.Interfaces;

public interface IAutocompleteApiClient
{
    /// <summary>
    /// Provides general info about requested city or country.
    /// </summary>
    /// <param name="name">City or country name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// <see cref="ILocation"/>
    /// </returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<ILocation?> GetLocationAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Provides general info about cities of requested country.
    /// </summary>
    /// <param name="countryName">City or country name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// List io <see cref="ILocation"/>
    /// </returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<IList<ILocation>> GetCountryLocationsAsync(string countryName, CancellationToken cancellationToken);
}