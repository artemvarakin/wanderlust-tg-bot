using System.Text;
using Wanderlust.Application.Departure.Dtos;
using Wanderlust.Application.Flights.Dtos;
using Wanderlust.Web.Interfaces;

namespace Wanderlust.Web.Services;

public class ReplyTextComposer : IReplyTextComposer
{
    public string GetUsageMessageText()
        => "Для поиска авиабилетов напишите боту город отправления.";

    public string GetSelectRegionText()
        => $"Выбирите регион поиска:";

    public string GetVisaFreeZoneFlightsText(DepartureBoardDto departureBoard)
    {
        var sb = new StringBuilder()
            // header
            .Append("<b>")
            .Append("Рейсы из ")
            .Append(departureBoard.DepartureCity.Cases.Ro)
            .Append(" в страны ")
            .Append(GetRegionGenitiveString(departureBoard.DirectionRegion))
            .Append('\n')
            .Append(departureBoard.Date.ToString("dd MMMM, dddd"))
            .AppendLine(":</b>\n");

        if (!departureBoard.Flights.Any())
        {
            sb.Append("Рейсы на эту дату не найдены.");
            return sb.ToString();
        }

        foreach (var flight in departureBoard.Flights.OrderBy(f => f.Price))
        {
            sb
                // destination
                .Append(flight.DestinationCity.Name)
                .Append(" (")
                .Append(flight.DestinationCity.Country.Name)
                .Append("): ")
                // price with link to ticket
                .Append("<a href=\"")
                .Append("https://www.aviasales.ru")
                .Append(flight.TicketLink)
                .Append("\">")
                .Append(flight.Price.ToString("C0"))
                .AppendLine("</a>");
        }

        return sb.ToString();
    }

    private string GetRegionGenitiveString(GeoRegionDto region)
    {
        return region switch
        {
            GeoRegionDto.Europe => "Европы",
            GeoRegionDto.Asia => "Азии",
            GeoRegionDto.MiddleEast => "Ближнего Востока",
            GeoRegionDto.Africa => "Африки",
            GeoRegionDto.NorthAmerica => "Северной Америки",
            GeoRegionDto.SouthAmerica => "Южной Америки",

            _ => throw new InvalidOperationException(
                $"Caught unhandled geo region: {region}")
        };
    }
}