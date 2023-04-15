using Telegram.Bot.Types;

namespace Wanderlust.Web.Interfaces;

public interface IUpdateHandler
{
    Task HandleAsync(Update update);
}