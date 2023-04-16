using Microsoft.Extensions.DependencyInjection;
using Wanderlust.TravelpayoutsClients.Clients;
using Wanderlust.TravelpayoutsClients.Interfaces;
using Wanderlust.TravelpayoutsClients.Services;

namespace Wanderlust.TravelpayoutsClients;

public static class DependencyInjection
{
    public static IServiceCollection AddTravelpayoutsDataApiClient(
        this IServiceCollection services, Uri uri, TimeSpan timeOut)
    {
        services.AddHttpClient<IDataApiClient, DataApiClient>(client =>
        {
            client.BaseAddress = uri;
            client.Timeout = timeOut;
        });

        return services;
    }

    public static IServiceCollection AddAutocompleteApiClient(
        this IServiceCollection services, Uri uri, TimeSpan timeOut)
    {
        services.AddHttpClient<IAutocompleteApiClient, AutocompleteApiClient>(client =>
        {
            client.BaseAddress = uri;
            client.Timeout = timeOut;
        });

        return services;
    }

    public static IServiceCollection AddAviasalesApiClient(
        this IServiceCollection services, Uri uri, string apiToken, TimeSpan timeOut)
    {
        services.AddHttpClient<IAviasalesApiClient, AviasalesApiClient>(httpClient =>
        {
            httpClient.BaseAddress = uri;
            httpClient.Timeout = timeOut;
            httpClient.DefaultRequestHeaders.Add("X-Access-Token", apiToken);
        });

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
