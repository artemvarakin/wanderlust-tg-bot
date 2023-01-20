using Telegram.Bot.Types;

namespace WanderlustTgBot.Abstractions;

public interface IUpdateProcessor
{
    Task ProcessTextMessageAsync(Message message);
    Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery);
}