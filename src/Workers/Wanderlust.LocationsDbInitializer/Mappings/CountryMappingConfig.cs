using Mapster;
using Wanderlust.Core.Domain;
using Wanderlust.TravelpayoutsClients.Models;

namespace Wanderlust.LocationsDbInitializer.Mappings;

public class CountryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CountryInfo, Country>()
            .Map(dest => dest.Id, _ => Guid.NewGuid());
    }
}