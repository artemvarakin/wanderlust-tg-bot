using Wanderlust.Core.Domain;

namespace Wanderlust.Application.Common.Interfaces;

public interface IAviasalesApiClient
{
    /// <summary>
    /// Return flights for next month.
    /// </summary>
    /// <param name="departureCode">Departure city code.</param>
    /// <param name="destinationCode">Destination region.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>
    /// <see cref="IEnumerable"/> of <see cref="AviasalesFlight"/>
    /// </returns>
    Task<IEnumerable<AviasalesFlight>> GetFlightsForMonthAsync(string departureCode, string destinationCode, CancellationToken cancellationToken);
}