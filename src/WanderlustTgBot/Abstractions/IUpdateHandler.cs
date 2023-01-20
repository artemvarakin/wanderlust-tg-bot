using Telegram.Bot.Types;

namespace WanderlustTgBot.Abstractions;

public interface IUpdateHandler
{
    Task HandleAsync(Update update);
}