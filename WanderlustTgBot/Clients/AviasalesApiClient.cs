using WanderlustTgBot.Abstractions;
using WanderlustTgBot.Data.Models;

namespace WanderlustTgBot.Clients;

public class AviasalesApiClient : IAviasalesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AviasalesApiClient(
        HttpClient httpClient,
        IDateTimeProvider dateTimeProvider)
    {
        _httpClient = httpClient;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsForMonthAsync(
        string cityCode,
        string destinationCode)
    {
        var utcNow = _dateTimeProvider.UtcNow;

        var result = await Task.WhenAll(
            GetFlightsForMonthAsync(cityCode, destinationCode, GetDepartureMonthString(utcNow)),
            GetFlightsForMonthAsync(cityCode, destinationCode, GetDepartureMonthString(utcNow.AddMonths(1)))
        );

        return result.SelectMany(f => f)
            .Where(f => f.DepartureAt <= utcNow.AddMonths(1));
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