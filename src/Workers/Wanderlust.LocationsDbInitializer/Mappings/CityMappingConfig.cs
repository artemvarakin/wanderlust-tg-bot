using Mapster;
using Wanderlust.Core.Domain;
using Wanderlust.TravelpayoutsClients.Models;

namespace Wanderlust.LocationsDbInitializer.Mappings;

public class CityMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(CityInfo cityInfo, Country country), City>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest, src => src.cityInfo)
            .Map(dest => dest.Country, src => src.country);
    }
}
