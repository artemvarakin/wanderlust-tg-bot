using Serilog;
using Telegram.Bot;
using Wanderlust.Web.Configurations;
using Wanderlust.Web.Interfaces;
using Wanderlust.Web.Services;

namespace Wanderlust.Web.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TgBotClientSettings>()
            .Bind(configuration.GetRequiredSection(
                TgBotClientSettings.SectionName))
            .Validate(o =>
                o.HostName is not null,
                "Telegram client host name missed.")
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.Token),
                "Telegram client token missed.")
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

            .AddSingleton<ICallbackQueryDataService, CallbackQueryDataService>()
            .AddSingleton<IMarkupService, MarkupService>()
            .AddSingleton<IDateProvider, DateProvider>()

            .AddScoped<IUpdateHandler, UpdateHandler>()
            .AddScoped<IUpdateProcessor, UpdateProcessor>()
            .AddScoped<IReplyService, ReplyService>()
            .AddScoped<IReplyTextComposer, ReplyTextComposer>();
    }
}