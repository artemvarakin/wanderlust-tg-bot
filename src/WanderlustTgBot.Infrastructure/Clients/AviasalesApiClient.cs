using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WanderlustTgBot.Infrastructure.Abstractions;

namespace WanderlustTgBot.Infrastructure.Clients;

public class AviasalesApiClient : IAviasalesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<AviasalesApiClient> _logger;

    public AviasalesApiClient(
        HttpClient httpClient,
        IDateTimeProvider dateTimeProvider,
        ILogger<AviasalesApiClient> logger)
    {
        _httpClient = httpClient;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsForMonthAsync(
        string cityCode,
        string destinationCode)
    {
        var utcNow = _dateTimeProvider.UtcNow;

        var requests = new[] {
            GetFlightsForMonthAsync(cityCode, destinationCode, GetDepartureMonthString(utcNow)),
            GetFlightsForMonthAsync(cityCode, destinationCode, GetDepartureMonthString(utcNow.AddMonths(1)))
        };

        IEnumerable<Flight>[]? results = null;
        try
        {
            results = await Task.WhenAll(requests);
        }
        catch
        {
            foreach (var request in requests.Where(r => r.Exception != null))
            {
                _logger.LogError(
                "Failed to get flights from {cityCode} to {destinationCode}: {message}",
                cityCode,
                destinationCode,
                request.Exception!.Message);
            }
        }

        return results is not null
            ? results.SelectMany(f => f).Where(f => f.DepartureAt <= utcNow.AddMonths(1))
            : Enumerable.Empty<Flight>();
    }

    private async Task<IEnumerable<Flight>> GetFlightsForMonthAsync(
        string cityCode,
        string destinationCode,
        string departureMonth)
    {
        var response = await _httpClient.GetFromJsonAsync<FlightsForMonth>(
            string.Format(
                "/aviasales/v3/prices_for_dates?origin={0}&destination={1}&departure_at={2}&limit=100",
                cityCode,
                destinationCode,
                departureMonth)
        );

        return response!.Data;
    }

    private static string GetDepartureMonthString(DateTime dateTime)
        => dateTime.ToString("yyyy-MM");
}