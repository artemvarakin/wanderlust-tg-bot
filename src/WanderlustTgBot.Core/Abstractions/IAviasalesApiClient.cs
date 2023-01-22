namespace WanderlustTgBot.Core.Abstractions;

public interface IAviasalesApiClient
{
    /// <summary>
    /// Gets flights for one month further.
    /// Since Aviasales API provides handler to fetch data only for specific month,
    /// this method have to call it twice (for current and next month)
    /// and then aggregates the result.
    /// </summary>
    /// <param name="departureCode">Departure city code.</param>
    /// <param name="destinationCode">Departure city or country code.</param>
    /// <returns>
    /// <see cref="IEnumerable"/> of <see cref="Flight"/>
    /// </returns>
    Task<IEnumerable<Flight>> GetFlightsForMonthAsync(string departureCode, string destinationCode);
}