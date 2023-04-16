using Wanderlust.Core.Domain.Abstractions;

namespace Wanderlust.TravelpayoutsClients.Models;

public record Location(
    Guid Id,
    string CityCode,
    string CityName,
    string CountryCode,
    string CountryName) : ILocation;
