using Microsoft.Extensions.Logging;
using Wanderlust.Core.Domain;
using Wanderlust.Application.Common.Interfaces;

namespace Wanderlust.Application.Common.Services;

public class FlightsProvider : IFlightsProvider
{
    private readonly ILogger<FlightsProvider> _logger;
    private readonly IAviasalesApiClient _client;
    private readonly IVisaFreeDirectionsRepository _visaFreeDirectionsRepository;
    private readonly ICitiesRepository _citiesRepository;

    public FlightsProvider(
        ILogger<FlightsProvider> logger,
        IAviasalesApiClient client,
        IVisaFreeDirectionsRepository visaFreeDirectionsRepository,
        ICitiesRepository citiesRepository)
    {
        _logger = logger;
        _client = client;
        _visaFreeDirectionsRepository = visaFreeDirectionsRepository;
        _citiesRepository = citiesRepository;
    }

    public async Task<IEnumerable<Flight>> GetFlightsForMonthAsync(
        City departureCity,
        GeoRegion destinationRegion,
        CancellationToken cancellationToken)
    {
        var directionCodes = await _visaFreeDirectionsRepository
            .GetDirectionCodesByRegionAsync(destinationRegion, cancellationToken);

        if (!directionCodes.Any())
        {
            throw new InvalidOperationException(
                $"Could not find any visa free direction code for region {destinationRegion}");
        }

        var result = await Task.WhenAll(directionCodes.Select(dc =>
            _client.GetFlightsForMonthAsync(departureCity.Code, dc, cancellationToken)));

        var data = result.SelectMany(f => f);

        if (!data.Any())
        {
            _logger.LogInformation(
                "Could not find any flight from {CityName} to {Region}.",
                departureCity.Cases.Ro,
                destinationRegion);

            return Enumerable.Empty<Flight>();
        }

        var cities = await _citiesRepository.GetCitiesByCodesAsync(
            data.Select(f => f.DestinationCode),
            cancellationToken);

        if (!cities.Any())
        {
            throw new InvalidOperationException(
                "Could not find any city in database.");
        }

        var citiesIdx = cities.ToDictionary(c => c.Code);

        return data.Select(f => new Flight(
            DepartureCity: departureCity,
            DestinationCity: citiesIdx[f.DestinationCode],
            DepartureAt: f.DepartureAt,
            NumberOfTransfers: f.NumberOfTransfers,
            Price: f.Price,
            TicketLink: f.TicketLink)
        );
    }
}
