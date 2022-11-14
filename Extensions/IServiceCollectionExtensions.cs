using Serilog;
using Telegram.Bot;
using WanderlustTgBot.Abstractions;
using WanderlustTgBot.Configurations;
using WanderlustTgBot.Services;

namespace WanderlustTgBot.Extensions;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // TODO: add fluent validation
        services.AddOptions<TelegramBotClientSettings>()
            .Bind(configuration.GetRequiredSection(TelegramBotClientSettings.SectionName));

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
        var token = configuration
            .GetRequiredSection(TelegramBotClientSettings.SectionName)
            .GetValue<string>("Token")
            ?? throw new InvalidOperationException("Telegram bot token missed.");

        services.AddHttpClient("tgwebhook").AddTypedClient<ITelegramBotClient>(HttpClient =>
            new TelegramBotClient(token, HttpClient));

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IUpdateHandler, UpdateHandler>();
    }
}