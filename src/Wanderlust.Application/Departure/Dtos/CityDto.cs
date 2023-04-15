namespace Wanderlust.Application.Departure.Dtos;

public record CityDto(
    string Name,
    string Code,
    CountryDto Country,
    CasesDto Cases);