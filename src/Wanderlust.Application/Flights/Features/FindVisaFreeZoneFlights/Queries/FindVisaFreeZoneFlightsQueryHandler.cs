using MapsterMapper;
using MediatR;
using Wanderlust.Core.Domain;
using Wanderlust.Application.Common.Interfaces;
using Wanderlust.Application.Flights.Dtos;

namespace Wanderlust.Application.Flights.Features.FindVisaFreeZoneFlights.Queries;

public class FindVisaFreeZoneFlightsQueryHandler
    : IRequestHandler<FindVisaFreeZoneFlightsQuery, DepartureBoardDto>
{
    private readonly IMapper _mapper;
    private readonly IDepartureBoardsCache _departureBoardsCache;
    private readonly IDepartureBoardsProvider _departureBoardsProvider;

    public FindVisaFreeZoneFlightsQueryHandler(
        IMapper mapper,
        IDepartureBoardsCache departureBoardCache,
        IDepartureBoardsProvider departureBoardsProvider)
    {
        _mapper = mapper;
        _departureBoardsCache = departureBoardCache;
        _departureBoardsProvider = departureBoardsProvider;
    }

    public async Task<DepartureBoardDto> Handle(
        FindVisaFreeZoneFlightsQuery query,
        CancellationToken cancellationToken)
    {
        var region = _mapper.Map<GeoRegion>(query.GeoRegion);

        // return if cached
        var cachedData = await _departureBoardsCache.GetDepartureBoardAsync(
            query.DepartureCode,
            region,
            query.Date);

        if (cachedData is not null)
        {
            return GetResult(cachedData);
        }

        // get data and cache it
        var departureBoards = await _departureBoardsProvider.GetDepartureBoardsAsync(
            query.DepartureCode,
            region,
            cancellationToken);

        await _departureBoardsCache.AddDepartureBoardsAsync(departureBoards, region);

        // return requested one
        return GetResult(departureBoards.First(x => x.Date == query.Date));
    }

    private DepartureBoardDto GetResult(DepartureBoard departureBoard)
    {
        return _mapper.Map<DepartureBoardDto>(departureBoard);
    }
}