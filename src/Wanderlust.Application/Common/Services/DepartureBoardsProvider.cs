using Wanderlust.Core.Domain;
using Wanderlust.Application.Common.Interfaces;

namespace Wanderlust.Application.Common.Services;

public class DepartureBoardsProvider : IDepartureBoardsProvider
{
    private readonly ICitiesRepository _citiesRepository;
    private readonly IFlightsProvider _flightsProvider;
    private readonly IDatesProvider _datesProvider;

    public DepartureBoardsProvider(
        ICitiesRepository citiesRepository,
        IFlightsProvider flightsProvider,
        IDatesProvider datesProvider)
    {
        _citiesRepository = citiesRepository;
        _flightsProvider = flightsProvider;
        _datesProvider = datesProvider;
    }

    public async Task<IEnumerable<DepartureBoard>> GetDepartureBoardsAsync(
        string departureCode,
        GeoRegion region,
        CancellationToken cancellationToken)
    {
        var departureCity = await _citiesRepository.GetCityByCodeAsync(
            departureCode,
            cancellationToken);

        if (departureCity is null)
        {
            throw new InvalidOperationException(
                $"Could not find departure city by code '{departureCode}'.");
        }

        var flights = await _flightsProvider.GetFlightsForMonthAsync(
            departureCity,
            region,
            cancellationToken);

        var flightsByDate = flights
            .GroupBy(f => DateOnly.FromDateTime(f.DepartureAt))
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        var dates = _datesProvider.GetDatesOneMonthAhead(
            DateOnly.FromDateTime(DateTime.UtcNow));

        return dates.Select(date =>
            new DepartureBoard(
                DepartureCity: departureCity,
                Date: date,
                DirectionRegion: region,
                Flights: flightsByDate.TryGetValue(date, out var flights)
                    ? flights
                    : Enumerable.Empty<Flight>()
            ));
    }
}
