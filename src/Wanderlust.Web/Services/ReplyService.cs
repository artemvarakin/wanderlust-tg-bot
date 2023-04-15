using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Wanderlust.Application.Flights.Dtos;
using Wanderlust.Web.Interfaces;

namespace Wanderlust.Web.Services;

public class ReplyService : IReplyService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMarkupService _markupService;
    private readonly IReplyTextComposer _replyTextComposer;

    public ReplyService(
        ITelegramBotClient botClient,
        IMarkupService markupService,
        IReplyTextComposer replyTextComposer)
    {
        _botClient = botClient;
        _markupService = markupService;
        _replyTextComposer = replyTextComposer;
    }

    public async Task SendUsageMessageAsync(Message message)
    {
        var text = _replyTextComposer.GetUsageMessageText();

        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: text);
    }

    public async Task SendSelectRegionMenuAsync(Message message, string departureCityCode)
    {
        var text = _replyTextComposer.GetSelectRegionText();
        var markup = _markupService.CreateSelectRegionMenuMarkup(departureCityCode);

        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: text,
            replyToMessageId: message.MessageId,
            replyMarkup: markup);
    }

    public async Task ReplyWithVisaFreeZoneFlightsAsync(
        Message message,
        DepartureBoardDto departureBoard)
    {
        var text = _replyTextComposer.GetVisaFreeZoneFlightsText(departureBoard);
        var markup = _markupService.CreateVisaFreeZoneMenuMarkup(departureBoard);

        await _botClient.EditMessageTextAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup);
    }

    public async Task BackToRequestSelectRegionMenuAsync(Message message, string departureCityCode)
    {
        var text = _replyTextComposer.GetSelectRegionText();
        var markup = _markupService.CreateSelectRegionMenuMarkup(departureCityCode);

        await _botClient.EditMessageTextAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            text: text,
            replyMarkup: markup);
    }

    public async Task CloseSelectRegionMenuAsync(Message message)
    {
        await _botClient.DeleteMessageAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId
        );
    }
}