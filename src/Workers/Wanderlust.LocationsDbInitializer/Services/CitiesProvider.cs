using MapsterMapper;
using Microsoft.Extensions.Logging;
using Wanderlust.Core.Domain;
using Wanderlust.LocationsDbInitializer.Interfaces;
using Wanderlust.TravelpayoutsClients.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class CitiesProvider : ICitiesProvider
{
    private readonly ILogger<CitiesProvider> _logger;
    private readonly IDataApiClient _client;
    private readonly IMapper _mapper;

    public CitiesProvider(
        ILogger<CitiesProvider> logger,
        IDataApiClient client,
        IMapper mapper)
    {
        _logger = logger;
        _client = client;
        _mapper = mapper;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(
        IEnumerable<Country> countries,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started fetching cities info from Travelpayouts API.");
        var citiesInfo = await _client.GetCitiesInfoAsync(cancellationToken);

        var idx = countries.ToDictionary(c => c.Code);

        return citiesInfo.Select(ci =>
            _mapper.Map<City>((ci, idx[ci.CountryCode])));
    }
}
