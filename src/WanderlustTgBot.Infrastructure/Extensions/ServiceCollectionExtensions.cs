using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using WanderlustTgBot.Core.Abstractions;
using WanderlustTgBot.Core.Configurations;
using WanderlustTgBot.Infrastructure.Clients;

namespace WanderlustTgBot.Infrastructure.Extensions;

internal static class IServiceCollectionExtensions
{
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

    public static IServiceCollection AddRedisCache(
        this IServiceCollection services)
    {
        return services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("Redis")!);
            }
        );
    }
}