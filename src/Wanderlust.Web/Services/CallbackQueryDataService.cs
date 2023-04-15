using Telegram.Bot.Types;
using Wanderlust.Application.Departure.Dtos;
using Wanderlust.Web.Interfaces;
using Wanderlust.Web.Models;

namespace Wanderlust.Web.Services;

public class CallbackQueryDataService : ICallbackQueryDataService
{
    private readonly ILogger<CallbackQueryDataService> _logger;

    public CallbackQueryDataService(
        ILogger<CallbackQueryDataService> logger)
    {
        _logger = logger;
    }

    public string GetVisaFreeZoneMenuData(string departureCode, GeoRegionDto region, DateOnly date)
    {
        return $"{CallbackQueryDataAction.VisaFreeZoneMenuNavigation}:{departureCode}:{region}:{date}";
    }

    public string GetSelectRegionMenuData(string departureCode)
    {
        return $"{CallbackQueryDataAction.BackToSelectRegionMenuRequest}:{departureCode}";
    }

    public string GetCloseSelectRegionMenuData()
    {
        return $"{CallbackQueryDataAction.CloseSelectRegionMenu}";
    }

    public ICallbackQueryData Parse(CallbackQuery callbackQuery)
    {
        try
        {
            if (callbackQuery.Data is null)
            {
                throw new InvalidOperationException("Callback query data missed.");
            }

            var data = callbackQuery.Data.Split(":");

            var action = Enum.Parse<CallbackQueryDataAction>(data[0]);

            return action switch
            {
                CallbackQueryDataAction.VisaFreeZoneMenuNavigation =>
                    new VisaFreeZoneFlightsMenuData(
                        DepartureCode: data[1],
                        Region: Enum.Parse<GeoRegionDto>(data[2]),
                        Date: DateOnly.Parse(data[3])),

                CallbackQueryDataAction.BackToSelectRegionMenuRequest =>
                    new BackToSelectRegionMenuData(DepartureCode: data[1]),

                CallbackQueryDataAction.CloseSelectRegionMenu =>
                    new CloseSelectRegionMenuData(),

                _ => throw new InvalidOperationException(
                    $"Hit unhandled callback query data action: {action}."
                )
            };
        }
        catch
        {
            _logger.LogError(
                "Failed to parse callback data received from user with username: '{Username}'.",
                callbackQuery.From.Username);

            throw;
        }
    }
}