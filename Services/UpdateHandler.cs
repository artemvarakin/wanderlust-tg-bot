using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WanderlustTgBot.Abstractions;

namespace WanderlustTgBot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        ILogger<UpdateHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(Update update)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => OnMessageReceivedAsync(update.Message!),
            UpdateType.EditedMessage => OnMessageReceivedAsync(update.EditedMessage!),
            _ => OnUnsupportedUpdateTypeAsync()
        };

        try
        {
            await handler;
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    private async Task OnMessageReceivedAsync(Message message)
    {
        if (message.Type != MessageType.Text
            || string.IsNullOrWhiteSpace(message.Text))
        {
            return;
        }
    }

    private async Task HandleErrorAsync(Exception e)
    {

    }

    private Task OnUnsupportedUpdateTypeAsync()
    {
        return Task.CompletedTask;
    }
}