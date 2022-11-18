using System.Net;
using WanderlustTgBot.Abstractions;
using WanderlustTgBot.Data.Models;

namespace WanderlustTgBot.Clients;

public class AutocompleteApiClient : IAutocompleteApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AutocompleteApiClient(IHttpClientFactory clientFactory)
    {
        _httpClientFactory = clientFactory;
    }

    ///<inheritdoc/>
    public async Task<DeparturePoint?> GetLocaleByCityAsync(string city)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(nameof(IAutocompleteApiClient));
            var result = await client.GetFromJsonAsync<IEnumerable<DeparturePoint>>(
                $"/places2?locale=en&term={city}");

            if (result is null || !result.Any())
            {
                return null;
            }

            return result.FirstOrDefault();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode
                is HttpStatusCode.BadRequest
                or HttpStatusCode.NotFound)
            {
                return null;
            }

            throw;
        }
    }
}