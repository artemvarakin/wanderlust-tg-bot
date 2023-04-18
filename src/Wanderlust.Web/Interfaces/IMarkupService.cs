using Telegram.Bot.Types.ReplyMarkups;
using Wanderlust.Application.Flights.Dtos;

namespace Wanderlust.Web.Interfaces;

public interface IMarkupService
{
    InlineKeyboardMarkup CreateSelectRegionMenuMarkup(string departureCode);
    InlineKeyboardMarkup CreateVisaFreeZoneMenuMarkup(DepartureBoardDto departureBoard);
}