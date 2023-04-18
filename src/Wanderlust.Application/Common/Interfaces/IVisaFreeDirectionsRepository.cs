using Wanderlust.Core.Domain;

namespace Wanderlust.Application.Common.Interfaces;

public interface IVisaFreeDirectionsRepository
{
    Task<IEnumerable<string>> GetDirectionCodesByRegionAsync(GeoRegion region, CancellationToken cancellationToken);
}