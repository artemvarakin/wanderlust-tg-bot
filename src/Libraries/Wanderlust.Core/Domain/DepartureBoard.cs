namespace Wanderlust.Core.Domain;

public record DepartureBoard(
    City DepartureCity,
    DateOnly Date,
    GeoRegion DirectionRegion,
    IEnumerable<Flight> Flights);