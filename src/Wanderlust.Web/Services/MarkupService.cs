using Telegram.Bot.Types.ReplyMarkups;
using Wanderlust.Application.Departure.Dtos;
using Wanderlust.Application.Flights.Dtos;
using Wanderlust.Web.Interfaces;

namespace Wanderlust.Web.Services;

public class MarkupService : IMarkupService
{
    private readonly ICallbackQueryDataService _callbackQueryDataService;
    private readonly IDateProvider _dateProvider;

    public MarkupService(
        ICallbackQueryDataService callbackQueryDataService,
        IDateProvider dateProvider)
    {
        _callbackQueryDataService = callbackQueryDataService;
        _dateProvider = dateProvider;
    }

    public InlineKeyboardMarkup CreateSelectRegionMenuMarkup(string departureCode)
    {
        var rows = new IEnumerable<InlineKeyboardButton>[]
        {
            // first row
            new[] {
                InlineKeyboardButton.WithCallbackData(
                    text: "Европа",
                    callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                        departureCode,
                        GeoRegionDto.Europe,
                        _dateProvider.Today)
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: "Азия",
                    callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                        departureCode,
                        GeoRegionDto.Asia,
                        _dateProvider.Today)
                )
            },
            // second row
            new[] {
                InlineKeyboardButton.WithCallbackData(
                    text: "Ближний Восток",
                    callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                        departureCode,
                        GeoRegionDto.MiddleEast,
                        _dateProvider.Today)
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: "Африка",
                    callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                        departureCode,
                        GeoRegionDto.Africa,
                        _dateProvider.Today)
                )
            },
            // third row
            new[] {
                InlineKeyboardButton.WithCallbackData(
                    text: "Северная Америка",
                    callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                        departureCode,
                        GeoRegionDto.NorthAmerica,
                        _dateProvider.Today)
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: "Южная Америка",
                    callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                        departureCode,
                        GeoRegionDto.SouthAmerica,
                        _dateProvider.Today)
                )
            },
            // forth row
            new[] {
                InlineKeyboardButton.WithCallbackData(
                    text: "Закрыть",
                    callbackData: _callbackQueryDataService.GetCloseSelectRegionMenuData()
                )
            }
        };

        return new InlineKeyboardMarkup(rows);
    }

    public InlineKeyboardMarkup CreateVisaFreeZoneMenuMarkup(DepartureBoardDto departureBoard)
    {
        var today = _dateProvider.Today;

        // date validation
        if (departureBoard.Date < today
            || departureBoard.Date > today.AddMonths(1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(departureBoard.Date), "Departure date is out of range of a month.");
        }

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        // pagination row
        var paginationRow = new List<InlineKeyboardButton>();

        if (departureBoard.Date > today)
        {
            paginationRow.Add(GetPaginationButton(
                "⬅️",
                departureBoard.DepartureCity.Code,
                departureBoard.DirectionRegion,
                departureBoard.Date.AddDays(-1))
            );
        }

        if (departureBoard.Date < today.AddMonths(1).AddDays(-1))
        {
            paginationRow.Add(GetPaginationButton(
                "➡️",
                departureBoard.DepartureCity.Code,
                departureBoard.DirectionRegion,
                departureBoard.Date.AddDays(1)));
        }

        rows.Add(paginationRow);

        // button to back to select region menu
        rows.Add(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData(
                text: "Назад",
                callbackData: _callbackQueryDataService.GetSelectRegionMenuData(
                    departureBoard.DepartureCity.Code)
            )
        });

        return new InlineKeyboardMarkup(rows);
    }

    private InlineKeyboardButton GetPaginationButton(
        string buttonText, string departureCode, GeoRegionDto region, DateOnly date)
    {
        return InlineKeyboardButton.WithCallbackData(
            text: buttonText,
            callbackData: _callbackQueryDataService.GetVisaFreeZoneMenuData(
                departureCode, region, date));
    }
}