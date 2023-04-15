using Wanderlust.Application.Departure.Dtos;

namespace Wanderlust.Application.Flights.Dtos;

public record DepartureBoardDto(
    CityDto DepartureCity,
    DateOnly Date,
    GeoRegionDto DirectionRegion,
    IEnumerable<FlightDto> Flights);