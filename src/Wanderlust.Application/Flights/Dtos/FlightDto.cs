namespace Wanderlust.Application.Flights.Dtos;

public record FlightDto(
    CityDto DepartureCity,
    CityDto DestinationCity,
    DateTime DepartureAt,
    int NumberOfTransfers,
    int Price,
    string TicketLink
);