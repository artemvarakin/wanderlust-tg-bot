using WanderlustTgBot.Application.Abstractions;

namespace WanderlustTgBot.Application.Services;

public class FlightsProvider : IFlightsProvider
{
    private readonly IScheduleCacheService _cache;
    private readonly IAviasalesRequestFactory _aviasalesRequestFactory;
    private readonly IFlightsProcessor _flightsProcessor;

    public FlightsProvider(
        IScheduleCacheService cache,
        IAviasalesRequestFactory aviasalesRequestFactory,
        IFlightsProcessor flightsProcessor)
    {
        _cache = cache;
        _aviasalesRequestFactory = aviasalesRequestFactory;
        _flightsProcessor = flightsProcessor;
    }

    public async Task<DepartureBoard> GetVisaFreeZoneFlights(FlightsSearchRequest searchRequest)
    {
        var cachedData = await _cache.GetVisaFreeFlightsAsync(searchRequest);
        if (cachedData is not null)
        {
            return cachedData;
        }

        var results = await Task.WhenAll(_aviasalesRequestFactory
            .CreateVisaFreeZoneRequests(searchRequest.DepartureCode));

        var schedule = _flightsProcessor
            .GetScheduleForEachDate(results.SelectMany(f => f));

        await _cache.AddScheduleAsync(
            searchRequest.DepartureCode,
            searchRequest.Region,
            schedule);

        return schedule.First(f => f.Date == searchRequest.Date);
    }
}