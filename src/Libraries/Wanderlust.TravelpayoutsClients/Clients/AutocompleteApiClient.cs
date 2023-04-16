using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wanderlust.Core.Domain.Abstractions;
using Wanderlust.TravelpayoutsClients.Interfaces;
using Wanderlust.TravelpayoutsClients.Models;

namespace Wanderlust.TravelpayoutsClients.Clients;

public class AutocompleteApiClient : IAutocompleteApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AutocompleteApiClient> _logger;

    public AutocompleteApiClient(
        HttpClient httpClient,
        ILogger<AutocompleteApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ILocation?> GetLocationAsync(
        string name, CancellationToken cancellationToken)
    {
        var body = await GetResponseBodyAsync(name, cancellationToken);

        return body.Any() ? ParseLocation(body.First()) : null;
    }

    public async Task<IList<ILocation>> GetCountryLocationsAsync(
        string countryName, CancellationToken cancellationToken)
    {
        var body = await GetResponseBodyAsync(countryName, cancellationToken);

        return body.Select(ParseLocation).ToList();
    }

    private async Task<IEnumerable<JsonElement>> GetResponseBodyAsync(
        string name, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetStreamAsync(
                $"/places2?locale=ru&types[]=city&term={name}",
                cancellationToken);

            var body = JsonSerializer.Deserialize<JsonElement>(response);

            return body.EnumerateArray();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode
                is HttpStatusCode.BadRequest
                or HttpStatusCode.NotFound)
            {
                _logger.LogInformation(
                    "Could not get info for '{name}': ({statusCode}).",
                    name,
                    e.StatusCode.Value);

                return Enumerable.Empty<JsonElement>();
            }

            throw;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize response body. '{name}' was requested.", e);
        }
    }

    private static ILocation ParseLocation(JsonElement data)
    {
        try
        {
            return new Location(
                Id: data.GetProperty("id").GetGuid(),
                CityCode: data.GetProperty("code").GetString()!,
                CityName: data.GetProperty("name").GetString()!,
                CountryCode: data.GetProperty("country_code").GetString()!,
                CountryName: data.GetProperty("country_name").GetString()!
            );
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                "Failed to parse location.", e);
        }
    }
}