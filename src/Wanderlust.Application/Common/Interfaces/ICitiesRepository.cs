using Wanderlust.Core.Domain;

namespace Wanderlust.Application.Common.Interfaces;

public interface ICitiesRepository
{
    Task<City?> GetCityByCodeAsync(string cityCode, CancellationToken cancellationToken);
    Task<IEnumerable<City>> GetCitiesByCodesAsync(IEnumerable<string> cityCodes, CancellationToken cancellationToken);
}