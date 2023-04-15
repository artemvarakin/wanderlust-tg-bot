using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using Wanderlust.Infrastructure.Persistence;
using Wanderlust.Application.Common.Interfaces;
using Wanderlust.Infrastructure.Clients;
using Wanderlust.Infrastructure.Configurations;
using Wanderlust.Infrastructure.Interfaces;
using Wanderlust.Infrastructure.Services;

namespace Wanderlust.Infrastructure.Extensions;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LocationsDatabaseSettings>()
            .Bind(configuration.GetRequiredSection(LocationsDatabaseSettings.SectionName))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.DatabaseName),
                "Locations database name missed.");

        services.AddOptions<AutocompleteApiClientSettings>()
            .Bind(configuration.GetRequiredSection(AutocompleteApiClientSettings.SectionName))
            .Validate(o =>
                o.Url is not null,
                "Autocomplete API URL missed.");

        services.AddOptions<AviasalesApiClientSettings>()
            .Bind(configuration.GetRequiredSection(AviasalesApiClientSettings.SectionName))
            .Validate(o =>
                o.Url is not null,
                "Aviasales API URL missed.")
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.ApiToken),
                "Aviasales API client token missed.");

        return services;
    }

    public static IServiceCollection AddHttpClients(
        this IServiceCollection services)
    {
        // Autocomplete API
        // Allows to fetch general info on specified city
        services.AddHttpClient<IAutocompleteApiClient, AutocompleteApiClient>((sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptions<AutocompleteApiClientSettings>>().Value;

            httpClient.BaseAddress = settings.Url;
            httpClient.Timeout = TimeSpan.FromSeconds(settings.RequestTimeout);
        });

        // Aviasales API
        services.AddHttpClient<IAviasalesApiClient, AviasalesApiClient>((sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptions<AviasalesApiClientSettings>>().Value;

            httpClient.BaseAddress = settings.Url;
            httpClient.Timeout = TimeSpan.FromSeconds(settings.RequestTimeout);
            httpClient.DefaultRequestHeaders.Add("X-Access-Token", settings.ApiToken);
        });

        return services;
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

    public static IServiceCollection ConfigureDepartureBoardsCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DepartureBoardsCache");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Departure Boards cache connection string messed.");
        }

        return services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return ConnectionMultiplexer.Connect(connectionString);
            }
        );
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddScoped<ICitiesRepository, CitiesRepository>()
            .AddScoped<IVisaFreeDirectionsRepository, VisaFreeDirectionsRepository>()
            .AddScoped<IDepartureBoardsCache, DepartureBoardCache>();
    }
}