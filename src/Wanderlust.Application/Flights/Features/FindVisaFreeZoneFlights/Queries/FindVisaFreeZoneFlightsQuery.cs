using MediatR;
using Wanderlust.Application.Departure.Dtos;
using Wanderlust.Application.Flights.Dtos;

namespace Wanderlust.Application.Flights.Features.FindVisaFreeZoneFlights.Queries;

public record FindVisaFreeZoneFlightsQuery(
    string DepartureCode,
    GeoRegionDto GeoRegion,
    DateOnly Date) : IRequest<DepartureBoardDto>;
