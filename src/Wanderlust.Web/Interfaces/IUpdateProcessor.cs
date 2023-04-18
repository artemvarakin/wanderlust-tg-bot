using Telegram.Bot.Types;

namespace Wanderlust.Web.Interfaces;

public interface IUpdateProcessor
{
    Task ProcessTextMessageAsync(Message message);
    Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery);
}