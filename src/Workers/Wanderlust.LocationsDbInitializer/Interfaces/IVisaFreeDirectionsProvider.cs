using Wanderlust.Core.Domain;

namespace Wanderlust.LocationsDbInitializer.Interfaces;

public interface IVisaFreeDirectionsProvider
{
    IEnumerable<VisaFreeDirection> GetVisaFreeDirections(IEnumerable<Country> countries);
}