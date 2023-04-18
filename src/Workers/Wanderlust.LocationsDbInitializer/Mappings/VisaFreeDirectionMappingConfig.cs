using Mapster;
using Wanderlust.Core.Domain;

namespace Wanderlust.LocationsDbInitializer.Mappings;

public class VisaFreeDirectionConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Country country, GeoRegion region), VisaFreeDirection>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.Country, src => src.country)
            .Map(dest => dest.Region, src => src.region);
    }
}
