using Wanderlust.Application.Departure.Dtos;

namespace Wanderlust.Web.Models;

public record VisaFreeZoneFlightsMenuData(
    string DepartureCode,
    GeoRegionDto Region,
    DateOnly Date) : ICallbackQueryData;