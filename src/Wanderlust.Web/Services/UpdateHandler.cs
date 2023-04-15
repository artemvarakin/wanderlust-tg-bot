using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Wanderlust.Web.Interfaces;

namespace Wanderlust.Web.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> _logger;
    private readonly IUpdateProcessor _updateProcessor;

    public UpdateHandler(
        ILogger<UpdateHandler> logger,
        IUpdateProcessor updateProcessor)
    {
        _logger = logger;
        _updateProcessor = updateProcessor;
    }

    public async Task HandleAsync(Update update)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => OnMessageReceivedAsync(update.Message!),
            UpdateType.EditedMessage => OnMessageReceivedAsync(update.EditedMessage!),
            UpdateType.CallbackQuery => OnCallbackQueryReceivedAsync(update.CallbackQuery!),
            _ => OnUnsupportedUpdateTypeAsync(update.Type)
        };

        try
        {
            await handler;
        }
        catch (Exception e)
        {
            HandleError(e);
        }
    }

    private async Task OnMessageReceivedAsync(Message message)
    {
        await _updateProcessor.ProcessTextMessageAsync(message);
    }

    private async Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
    {
        await _updateProcessor.ProcessCallbackQueryAsync(callbackQuery);
    }

    private void HandleError(Exception e)
    {
        _logger.LogCritical(e, "Failed to handle update.");
    }

    private Task OnUnsupportedUpdateTypeAsync(UpdateType updateType)
    {
        _logger.LogWarning("Received unsupported update type '{Type}'", updateType);

        return Task.CompletedTask;
    }
}