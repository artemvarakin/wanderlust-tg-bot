using Telegram.Bot.Types;
using Wanderlust.Application.Departure.Dtos;
using Wanderlust.Web.Models;

namespace Wanderlust.Web.Interfaces;

public interface ICallbackQueryDataService
{
    string GetVisaFreeZoneMenuData(string departureCode, GeoRegionDto region, DateOnly date);
    string GetSelectRegionMenuData(string departureCode);
    string GetCloseSelectRegionMenuData();
    ICallbackQueryData Parse(CallbackQuery callbackQuery);
}