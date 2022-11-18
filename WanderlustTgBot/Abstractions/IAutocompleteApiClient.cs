using WanderlustTgBot.Data.Models;

namespace WanderlustTgBot.Abstractions;

public interface IAutocompleteApiClient
{
    /// <summary>
    /// Gets <see cref="DeparturePoint"/> for provided city name.
    /// If a country name is provided,
    /// returns <see cref="DeparturePoint"/> of its capital.
    /// </summary>
    /// <param name="city">City name.</param>
    /// <returns>
    /// <see cref="DeparturePoint"/> or null
    /// if invalid value provided
    /// or city could not be found.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when Autocomplete API is unavailable.
    /// </exception>
    Task<DeparturePoint?> GetLocaleByCityAsync(string city);
}