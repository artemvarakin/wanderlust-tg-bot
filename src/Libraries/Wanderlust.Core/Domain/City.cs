using Wanderlust.Core.Domain.Abstractions;

namespace Wanderlust.Core.Domain;

public record City(
    Guid Id,
    string Name,
    string Code,
    Country Country,
    Cases Cases) : ILocation;