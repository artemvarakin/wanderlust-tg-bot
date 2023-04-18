namespace Wanderlust.Application.Common.Interfaces;

public interface IAutocompleteApiClient
{
    Task<string?> GetCityCodeByNameAsync(string cityName, CancellationToken cancellationToken);
}