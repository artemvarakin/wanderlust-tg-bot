using Wanderlust.Core.Domain;

namespace Wanderlust.Application.Common.Interfaces;

public interface IDepartureBoardsCache
{
    Task<DepartureBoard?> GetDepartureBoardAsync(string departureCode, GeoRegion region, DateOnly date);
    Task AddDepartureBoardsAsync(IEnumerable<DepartureBoard> departureBoards, GeoRegion region);
}