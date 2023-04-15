namespace Wanderlust.Core.Domain;

public record AviasalesFlight(
    string DepartureCode,
    string DestinationCode,
    DateTime DepartureAt,
    int NumberOfTransfers,
    int Price,
    string TicketLink);