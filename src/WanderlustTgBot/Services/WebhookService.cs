using Microsoft.Extensions.Options;
using Telegram.Bot;
using WanderlustTgBot.Infrastructure.Configurations;

namespace WanderlustTgBot.Services;

public class WebhookService : IHostedService
{
    private readonly ILogger<WebhookService> _logger;
    private readonly IServiceScopeFactory _factory;
    private readonly TelegramBotClientSettings _botSettings;

    public WebhookService(
        ILogger<WebhookService> logger,
        IServiceScopeFactory factory,
        IOptions<TelegramBotClientSettings> options)
    {
        _logger = logger;
        _factory = factory;
        _botSettings = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var botClient = GetBotClient();

        var webhookAddress = $"{_botSettings.HostName}/{_botSettings.Token}";
        _logger.LogInformation(
            "Setting webhook for the bot host name: {hostname}.",
            _botSettings.HostName);

        await botClient.SetWebhookAsync(
            webhookAddress,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var botClient = GetBotClient();

        _logger.LogInformation("Removing webhook.");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Application is shutting down.");
    }

    private ITelegramBotClient GetBotClient()
    {
        using var scope = _factory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
    }
}