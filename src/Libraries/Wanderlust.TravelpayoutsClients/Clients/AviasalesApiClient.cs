using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wanderlust.TravelpayoutsClients.Interfaces;
using Wanderlust.TravelpayoutsClients.Models;

namespace Wanderlust.TravelpayoutsClients.Clients;

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

    public async Task<IEnumerable<AviasalesFlight>> GetFlightsForMonthAsync(
        string departureCode,
        string destinationCode,
        CancellationToken cancellationToken)
    {
        var utcNow = _dateTimeProvider.UtcNow;
        var currentMonth = GetDepartureMonth(utcNow);
        var nextMonth = GetDepartureMonth(utcNow.AddMonths(1));

        var requests = new[] {
            GetFlightsForMonthAsync(departureCode, destinationCode, currentMonth, cancellationToken),
            GetFlightsForMonthAsync(departureCode, destinationCode, nextMonth, cancellationToken)
        };

        IEnumerable<AviasalesFlight>[]? results = null;
        try
        {
            results = await Task.WhenAll(requests);
        }
        catch
        {
            foreach (var request in requests.Where(t => t.Exception is not null))
            {
                _logger.LogError(
                "Failed to get flights from {cityCode} to {destinationCode}: {message}",
                departureCode,
                destinationCode,
                request.Exception!.Message);
            }
        }

        return results is not null
            ? results.SelectMany(f => f).Where(f => f.DepartureAt <= utcNow.AddMonths(1))
            : Enumerable.Empty<AviasalesFlight>();
    }

    private async Task<IEnumerable<AviasalesFlight>> GetFlightsForMonthAsync(
        string departureCode,
        string destinationCode,
        string departureMonth,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetStringAsync(
            string.Format(
                "/aviasales/v3/prices_for_dates?origin={0}&destination={1}&departure_at={2}&limit=100",
                departureCode,
                destinationCode,
                departureMonth),
            cancellationToken
        );

        return GetFlightsFromResponse(response);
    }

    private static IEnumerable<AviasalesFlight> GetFlightsFromResponse(string response)
    {
        var body = JsonSerializer.Deserialize<JsonElement>(response);

        foreach (var chunk in body.GetProperty("data").EnumerateArray())
        {
            yield return new AviasalesFlight(
                DepartureCode: chunk.GetProperty("origin").GetString()!,
                DestinationCode: chunk.GetProperty("destination").GetString()!,
                DepartureAt: chunk.GetProperty("departure_at").GetDateTime(),
                NumberOfTransfers: chunk.GetProperty("transfers").GetInt32(),
                Price: chunk.GetProperty("price").GetInt32(),
                TicketLink: chunk.GetProperty("link").GetString()!
            );
        }
    }

    private static string GetDepartureMonth(DateTime dateTime)
        => dateTime.ToString("yyyy-MM");
}