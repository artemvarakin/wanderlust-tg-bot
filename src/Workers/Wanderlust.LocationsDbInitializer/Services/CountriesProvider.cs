using MapsterMapper;
using Microsoft.Extensions.Logging;
using Wanderlust.Core.Domain;
using Wanderlust.LocationsDbInitializer.Interfaces;
using Wanderlust.TravelpayoutsClients.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class CountriesProvider : ICountriesProvider
{
    private readonly ILogger<CountriesProvider> _logger;
    private readonly IDataApiClient _client;
    private readonly IMapper _mapper;

    public CountriesProvider(
        ILogger<CountriesProvider> logger,
        IDataApiClient dataApiClient,
        IMapper mapper)
    {
        _logger = logger;
        _client = dataApiClient;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Country>> GetCountriesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started fetching countries info from Travelpayouts API.");
        var countriesInfo = await _client.GetCountriesInfoAsync(cancellationToken);

        return countriesInfo.Select(_mapper.Map<Country>);
    }
}
