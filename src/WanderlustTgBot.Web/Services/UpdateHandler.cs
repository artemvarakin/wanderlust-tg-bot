using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WanderlustTgBot.Web.Abstractions;

namespace WanderlustTgBot.Web.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly IUpdateProcessor _updateProcessor;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        IUpdateProcessor updateProcessor,
        ILogger<UpdateHandler> logger)
    {
        _updateProcessor = updateProcessor;
        _logger = logger;
    }

    public async Task HandleAsync(Update update)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => OnMessageReceivedAsync(update.Message!),
            UpdateType.EditedMessage => OnMessageReceivedAsync(update.EditedMessage!),
            UpdateType.CallbackQuery => OnCallbackQueryReceivedAsync(update.CallbackQuery!),
            _ => OnUnsupportedUpdateTypeAsync()
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
        _logger.LogError(e, "Error occurred: {message}.", e.Message);
    }

    private static Task OnUnsupportedUpdateTypeAsync()
    {
        // todo: log info
        return Task.CompletedTask;
    }
}