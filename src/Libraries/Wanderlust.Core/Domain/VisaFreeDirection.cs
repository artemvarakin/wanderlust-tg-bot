using Wanderlust.Core.Domain.Abstractions;

namespace Wanderlust.Core.Domain;

public record VisaFreeDirection(
    Guid Id,
    Country Country,
    GeoRegion Region) : ILocation;