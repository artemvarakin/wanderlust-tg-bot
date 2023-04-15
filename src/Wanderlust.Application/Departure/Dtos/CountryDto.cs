namespace Wanderlust.Application.Departure.Dtos;

public record CountryDto(
    string Name,
    string Code,
    CasesDto Cases);