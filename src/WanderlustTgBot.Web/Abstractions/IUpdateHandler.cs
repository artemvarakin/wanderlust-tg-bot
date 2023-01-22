using Telegram.Bot.Types;

namespace WanderlustTgBot.Web.Abstractions;

public interface IUpdateHandler
{
    Task HandleAsync(Update update);
}