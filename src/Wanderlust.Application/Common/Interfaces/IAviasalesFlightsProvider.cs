using Wanderlust.Core.Domain;

namespace Wanderlust.Application.Common.Interfaces;

public interface IFlightsProvider
{
    Task<IEnumerable<Flight>> GetFlightsForMonthAsync(City departureCity, GeoRegion destinationRegion, CancellationToken cancellationToken);
}