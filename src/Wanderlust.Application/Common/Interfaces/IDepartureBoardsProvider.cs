using Wanderlust.Core.Domain;

namespace Wanderlust.Application.Common.Interfaces;

public interface IDepartureBoardsProvider
{
    Task<IEnumerable<DepartureBoard>> GetDepartureBoardsAsync(string departureCode, GeoRegion region, CancellationToken cancellationToken);
}