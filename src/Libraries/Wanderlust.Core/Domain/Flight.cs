namespace Wanderlust.Core.Domain;

public record Flight(
    City DepartureCity,
    City DestinationCity,
    DateTime DepartureAt,
    int NumberOfTransfers,
    int Price,
    string TicketLink);