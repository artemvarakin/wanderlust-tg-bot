using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using Telegram.Bot;
using WanderlustTgBot.Abstractions;
using WanderlustTgBot.Infrastructure.Abstractions;
using WanderlustTgBot.Infrastructure.Clients;
using WanderlustTgBot.Infrastructure.Configurations;
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

        services.AddOptions<AutocompleteApiClientSettings>()
            .Bind(configuration.GetRequiredSection(AutocompleteApiClientSettings.SectionName));

        services.AddOptions<AviasalesApiClientSettings>()
            .Bind(configuration.GetRequiredSection(AviasalesApiClientSettings.SectionName));

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
            .AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(token!, client));

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
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Redis")!));
    }

    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services)
    {
        return services
            .AddScoped<IUpdateHandler, UpdateHandler>()
            .AddScoped<IUpdateProcessor, UpdateProcessor>();
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services)
    {
        return services;
    }
    // public static IServiceCollection AddServices(this IServiceCollection services)
    // {
    //     return services
    //         .AddSingleton<IDirectionsRepository, DirectionsRepository>()
    //         .AddScoped<IReplyService, ReplyService>()
    //         .AddScoped<IReplyComposer, ReplyComposer>()
    //         .AddScoped<IDateTimeProvider, DateTimeProvider>()
    //         .AddScoped<IScheduleCacheService, ScheduleCacheService>()
    //         .AddScoped<ICallbackDataService, CallbackDataService>()
    //         .AddScoped<IFlightsProvider, FlightsProvider>()
    //         .AddScoped<IFlightsProcessor, FlightsProcessor>()
    //         .AddScoped<IAviasalesRequestFactory, AviasalesRequestFactory>();
    // }
}