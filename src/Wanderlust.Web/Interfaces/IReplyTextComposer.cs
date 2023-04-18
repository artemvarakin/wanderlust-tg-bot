using Wanderlust.Application.Departure.Dtos;
using Wanderlust.Application.Flights.Dtos;

namespace Wanderlust.Web.Interfaces;

public interface IReplyTextComposer
{
    string GetUsageMessageText();
    string GetSelectRegionText();
    string GetVisaFreeZoneFlightsText(DepartureBoardDto departureBoard);
}