using Telegram.Bot;
using Telegram.Bot.Types;
using WanderlustTgBot.Web.Abstractions;

namespace WanderlustTgBot.Web.Services;

public class ReplyService : IReplyService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IReplyComposer _replyComposer;

    public ReplyService(
        ITelegramBotClient botClient,
        IReplyComposer replyComposer)
    {
        _botClient = botClient;
        _replyComposer = replyComposer;
    }

    public async Task ReplyWithUsageMessage(Message message)
    {
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: _replyComposer.GetUsageMessage()
        );
    }

    public async Task RequestSearchTypeAsync(Message message, string departureCode)
    {
        var markup = Markup.CreateSearchTypeMarkup(departureCode);

        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Выберите тип поиска:",
            replyToMessageId: message.MessageId,
            replyMarkup: markup
        );
    }
}