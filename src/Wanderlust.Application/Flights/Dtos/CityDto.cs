namespace Wanderlust.Application.Flights.Dtos;

public record CityDto(
    string Name,
    string Code,
    CountryDto Country,
    CasesDto Cases);