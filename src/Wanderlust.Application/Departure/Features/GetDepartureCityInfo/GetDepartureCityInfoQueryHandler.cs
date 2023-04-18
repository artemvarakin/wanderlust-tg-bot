using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Wanderlust.Application.Common.Interfaces;
using Wanderlust.Application.Departure.Dtos;

namespace Wanderlust.Application.Departure.Features.GetDepartureCityInfo;

public class GetDepartureCityInfoQueryHandler :
    IRequestHandler<GetDepartureCityInfoQuery, CityDto?>
{
    private readonly ILogger<GetDepartureCityInfoQueryHandler> _logger;
    private readonly IAutocompleteApiClient _client;
    private readonly ICitiesRepository _citiesRepository;
    private readonly IMapper _mapper;

    public GetDepartureCityInfoQueryHandler(
        ILogger<GetDepartureCityInfoQueryHandler> logger,
        IAutocompleteApiClient client,
        ICitiesRepository repository,
        IMapper mapper)
    {
        _logger = logger;
        _client = client;
        _citiesRepository = repository;
        _mapper = mapper;
    }

    public async Task<CityDto?> Handle(
        GetDepartureCityInfoQuery query,
        CancellationToken cancellationToken)
    {
        var cityCode = await _client.GetCityCodeByNameAsync(
            query.CityName,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(cityCode))
        {
            return null;
        }

        var city = await _citiesRepository.GetCityByCodeAsync(cityCode, cancellationToken);

        if (city is null)
        {
            _logger.LogWarning(
                "Could not find city with code '{code}' in Locations database.",
                cityCode);

            return null;
        }

        return _mapper.Map<CityDto>(city);
    }
}
