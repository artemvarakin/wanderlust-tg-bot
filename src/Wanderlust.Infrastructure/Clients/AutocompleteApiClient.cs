using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wanderlust.Application.Common.Interfaces;

namespace Wanderlust.Infrastructure.Clients;

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

    public async Task<string?> GetCityCodeByNameAsync(
        string cityName,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetStreamAsync(
                $"/places2?locale=ru&types[]=city&term={cityName}",
                cancellationToken);

            return GetCityCodeFromResponse(response);
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode
                is HttpStatusCode.BadRequest
                or HttpStatusCode.NotFound)
            {
                _logger.LogInformation(
                    "Could not get info for requested city '{City}' ({StatusCode}).",
                    cityName,
                    e.StatusCode.Value);

                return null;
            }

            throw;
        }
    }

    private static string? GetCityCodeFromResponse(Stream response)
    {
        var body = JsonSerializer.Deserialize<JsonElement>(response);
        var data = body.EnumerateArray().FirstOrDefault();

        return data.ValueKind != JsonValueKind.Object
            ? string.Empty
            : data.GetProperty("code").GetString();
    }
}