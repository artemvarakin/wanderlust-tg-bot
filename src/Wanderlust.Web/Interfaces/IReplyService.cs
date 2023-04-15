using Telegram.Bot.Types;
using Wanderlust.Application.Flights.Dtos;

namespace Wanderlust.Web.Interfaces;

public interface IReplyService
{
    Task SendUsageMessageAsync(Message message);
    Task SendSelectRegionMenuAsync(Message message, string departureCityCode);
    Task ReplyWithVisaFreeZoneFlightsAsync(Message message, DepartureBoardDto departureBoard);
    Task BackToRequestSelectRegionMenuAsync(Message message, string departureCityCode);
    Task CloseSelectRegionMenuAsync(Message message);
}