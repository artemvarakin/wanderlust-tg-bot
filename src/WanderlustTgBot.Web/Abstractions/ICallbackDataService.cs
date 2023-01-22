using Telegram.Bot.Types;

namespace WanderlustTgBot.Web.Abstractions;

public interface ICallbackDataService
{
    FlightsSearchRequest ParseFlightsSearchRequest(CallbackQuery callbackQuery);
}