using Microsoft.Extensions.Logging;
using Wanderlust.LocationsDbInitializer.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class ExecutionService : IExecutionService
{
    private readonly ILogger<ExecutionService> _logger;
    private readonly ICountriesProvider _countriesProvider;
    private readonly ICitiesProvider _citiesProvider;
    private readonly IVisaFreeDirectionsProvider _visaFreeDirectionsProvider;
    private readonly ILocationsDbManager _locationsDbManager;

    public ExecutionService(
        ILogger<ExecutionService> logger,
        ICountriesProvider countriesProvider,
        ICitiesProvider citiesProvider,
        IVisaFreeDirectionsProvider visaFreeDirectionsProvider,
        ILocationsDbManager locationsDbManager)
    {
        _logger = logger;
        _countriesProvider = countriesProvider;
        _citiesProvider = citiesProvider;
        _visaFreeDirectionsProvider = visaFreeDirectionsProvider;
        _locationsDbManager = locationsDbManager;
    }

    public async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            var countries = await _countriesProvider.GetCountriesAsync(cancellationToken);

            var cities = await _citiesProvider.GetCitiesAsync(countries, cancellationToken);
            await _locationsDbManager.PushDataAsync(cities, cancellationToken);

            var visaFreeDirections = _visaFreeDirectionsProvider.GetVisaFreeDirections(countries);
            await _locationsDbManager.PushDataAsync(visaFreeDirections, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Locations DB initializer run failed.");

            throw;
        }
    }
}
