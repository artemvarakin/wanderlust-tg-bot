using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using WanderlustTgBot.Abstractions;
using WanderlustTgBot.Clients;
using WanderlustTgBot.Configurations;
using WanderlustTgBot.Services;

namespace WanderlustTgBot.Extensions;

internal static class IServiceCollectionExtensions
{
    // TODO: add fluent validation
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TelegramBotClientSettings>()
            .Bind(configuration.GetRequiredSection(TelegramBotClientSettings.SectionName));

        services.AddOptions<ClientSettings>()
            .Bind(configuration.GetRequiredSection(ClientSettings.SectionName));

        return services;
    }

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

    public static IServiceCollection ConfigureHttpClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Telegram bot client
        var token = configuration
            .GetRequiredSection(TelegramBotClientSettings.SectionName)
            .GetValue<string>("Token");

        services.AddHttpClient(nameof(ITelegramBotClient))
            .AddTypedClient<ITelegramBotClient>(HttpClient => new TelegramBotClient(token!, HttpClient));

        // Autocomplete API.
        // Allows to fetch general info on specified city.
        services.AddHttpClient(nameof(IAutocompleteApiClient), (sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptions<ClientSettings>>().Value;
            httpClient.BaseAddress = settings.AutocompleteApiUrl;
        });

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IUpdateHandler, UpdateHandler>()
            .AddScoped<IAutocompleteApiClient, AutocompleteApiClient>();
    }
}