using System.Net;
using System.Text.Json.Nodes;
using WanderlustTgBot.Infrastructure.Abstractions;

namespace WanderlustTgBot.Infrastructure.Clients;

public class AutocompleteApiClient : IAutocompleteApiClient
{
    private readonly HttpClient _httpClient;

    public AutocompleteApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<string?> GetDepartureCodeByNameAsync(string cityName)
    {
        try
        {
            var response = await _httpClient.GetStreamAsync(
                $"/places2?locale=en&term={cityName}");

            var node = JsonNode.Parse(response);

            // todo: remember why
            try
            {
                return node![0]!["code"]!.GetValue<string>();
            }
            catch
            {
                return null;
            }
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