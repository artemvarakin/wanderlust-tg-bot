using System.Runtime.Serialization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wanderlust.TravelpayoutsClients.Interfaces;
using Wanderlust.TravelpayoutsClients.Models;
using JorgeSerrano.Json;

namespace Wanderlust.TravelpayoutsClients.Clients;

public class DataApiClient : IDataApiClient
{
    private readonly ILogger<DataApiClient> _logger;
    private readonly HttpClient _client;

    public DataApiClient(ILogger<DataApiClient> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IEnumerable<CountryInfo>> GetCountriesInfoAsync(CancellationToken cancellationToken)
    {
        return await GetManyAsync<CountryInfo>("/data/ru/countries.json", cancellationToken);
    }


    public async Task<IEnumerable<CityInfo>> GetCitiesInfoAsync(CancellationToken cancellationToken)
    {
        return await GetManyAsync<CityInfo>("/data/ru/cities.json", cancellationToken);
    }

    private async Task<IEnumerable<T>> GetManyAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
            };

            var result = await _client.GetFromJsonAsync<IEnumerable<T>>(
                requestUri, options, cancellationToken);

            if (result is null)
            {
                throw new Exception("Failed to deserialize response.");
            }

            return result;
        }
        catch
        {
            _logger.LogError(
                "Could not fetch {Type} data from Travelpayouts API.",
                typeof(T).Name);

            throw;
        }
    }
}