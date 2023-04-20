using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
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

        services.AddOptions<ElasticsearchSettings>()
            .Bind(configuration.GetRequiredSection(
                ElasticsearchSettings.SectionName))
            .Validate(o =>
                o.Uri is not null,
                "Elasticsearch Uri missed.")
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.ApplicationName),
                "Application name missed.")
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.Username),
                "Username missed.")
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.Password),
                "Password missed.")
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection ConfigureLogging(
        this IServiceCollection services,
        IConfiguration configuration, string environmentName)
    {
        return services.AddLogging(builder =>
            {
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Environment", environmentName)
                    .WriteTo.Elasticsearch(
                        GetSinkOptions(configuration, environmentName))
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
            .GetValue<string>(nameof(TgBotClientSettings.Token));

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

    private static ElasticsearchSinkOptions GetSinkOptions(IConfiguration configuration, string environmentName)
    {
        var settings = configuration
            .GetRequiredSection(ElasticsearchSettings.SectionName);

        return new ElasticsearchSinkOptions(settings.GetValue<Uri>(nameof(ElasticsearchSettings.Uri)))
        {
            IndexFormat = $"{settings.GetValue<string>(nameof(ElasticsearchSettings.ApplicationName))}" +
                $"-logs-{environmentName.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            AutoRegisterTemplate = true,
            ModifyConnectionSettings = c => c.BasicAuthentication(
                username: settings.GetValue<string>(nameof(ElasticsearchSettings.Username)),
                password: settings.GetValue<string>(nameof(ElasticsearchSettings.Password))
            )
        };
    }
}