namespace Wanderlust.Application.Flights.Dtos;

public record CountryDto(
    string Name,
    string Code,
    CasesDto Cases);