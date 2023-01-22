using Serilog;
using Telegram.Bot;
using WanderlustTgBot.Core.Configurations;
using WanderlustTgBot.Web.Abstractions;
using WanderlustTgBot.Web.Services;

namespace WanderlustTgBot.Extensions;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TgBotClientSettings>()
            .Bind(configuration.GetRequiredSection(
                TgBotClientSettings.SectionName))
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.Token),
                "Telegram client token missed.")
            .ValidateOnStart();

        services.AddOptions<AutocompleteApiClientSettings>()
            .Bind(configuration.GetSection(
                AutocompleteApiClientSettings.SectionName))
            .ValidateOnStart();

        services.AddOptions<AviasalesApiClientSettings>()
            .Bind(configuration.GetSection(
                AviasalesApiClientSettings.SectionName))
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.ApiToken),
                "Aviasales API client token missed.")
            .ValidateOnStart();

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
            }
        );
    }

    public static IServiceCollection ConfigureTelegramBotHttpClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var token = configuration
            .GetRequiredSection(TgBotClientSettings.SectionName)
            .GetValue<string>("Token");

        services.AddHttpClient(nameof(ITelegramBotClient))
            .AddTypedClient<ITelegramBotClient>(client
                => new TelegramBotClient(token!, client));

        return services;
    }

    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services)
    {
        return services
            .AddHostedService<WebhookService>()
            .AddScoped<IUpdateHandler, UpdateHandler>()
            .AddScoped<IUpdateProcessor, UpdateProcessor>()
            .AddScoped<IReplyService, ReplyService>()
            .AddScoped<IReplyComposer, ReplyComposer>();
    }
}