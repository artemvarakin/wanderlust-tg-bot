using Telegram.Bot.Types;

namespace WanderlustTgBot.Web.Abstractions;

public interface IUpdateProcessor
{
    Task ProcessTextMessageAsync(Message message);
    Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery);
}