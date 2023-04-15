using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;
using Wanderlust.Core.Domain;
using Wanderlust.LocationsDbInitializer.Configurations;
using Wanderlust.LocationsDbInitializer.Data.Repositories;
using Wanderlust.LocationsDbInitializer.Interfaces;
using Wanderlust.LocationsDbInitializer.Services;
using Wanderlust.TravelpayoutsClients;

namespace Wanderlust.LocationsDbInitializer.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddLogging(builder =>
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            builder
                .ClearProviders()
                .AddSerilog(logger);
        });
    }

    public static IServiceCollection AddDataMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        return services
            .AddSingleton(config)
            .AddScoped<IMapper, ServiceMapper>();
    }

    public static IServiceCollection ConfigureTravelpayoutsDataApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetRequiredSection(TravelpayoutsDataApiClientSettings.SectionName)
            .Get<TravelpayoutsDataApiClientSettings>();

        if (options is null)
        {
            throw new InvalidOperationException(
                $"Could not bind {nameof(TravelpayoutsDataApiClientSettings)}.");
        }

        return services.AddTravelpayoutsDataApiClient(
            options.Url,
            TimeSpan.FromSeconds(options.RequestTimeout));
    }

    public static IServiceCollection ConfigureLocationsDbClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LocationsDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Locations database connection string missed.");
        }

        return services
            .AddScoped<IMongoClient>(_ => new MongoClient(connectionString));
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddRepositories()
            .AddScoped<IExecutionService, ExecutionService>()
            .AddScoped<ICountriesProvider, CountriesProvider>()
            .AddScoped<ICitiesProvider, CitiesProvider>()
            .AddScoped<IVisaFreeDirectionsProvider, VisaFreeDirectionsProvider>()
            .AddScoped<IVisaFreeDirectionsListReader, VisaFreeDirectionsListReader>()
            .AddScoped<ILocationsDbManager, LocationsDbManager>()
            .AddScoped<ILocationRepositoryFactory, LocationRepositoryFactory>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<BaseLocationsRepository<City>, CitiesRepository>()
            .AddScoped<BaseLocationsRepository<VisaFreeDirection>, VisaFreeDirectionsRepository>();
    }
}