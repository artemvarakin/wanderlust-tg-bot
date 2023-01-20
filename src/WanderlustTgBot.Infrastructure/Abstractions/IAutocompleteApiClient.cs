
namespace WanderlustTgBot.Infrastructure.Abstractions;
public interface IAutocompleteApiClient
{
    /// <summary>
    /// Gets IATA code for provided city name.
    /// If a country name is provided,
    /// returns IATA code of its capital.
    /// </summary>
    /// <param name="cityName">City name.</param>
    /// <returns>
    /// <see cref="string"/> representation of IATA code
    /// or null if invalid value provided
    /// or city not found.
    /// </returns>
    Task<string?> GetDepartureCodeByNameAsync(string cityName);
}