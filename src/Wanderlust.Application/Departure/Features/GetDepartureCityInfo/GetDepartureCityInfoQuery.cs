using MediatR;
using Wanderlust.Application.Departure.Dtos;

namespace Wanderlust.Application.Departure.Features.GetDepartureCityInfo;

public record GetDepartureCityInfoQuery(string CityName)
    : IRequest<CityDto?>;