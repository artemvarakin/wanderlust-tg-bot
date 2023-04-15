namespace Wanderlust.Core.Domain;

public record Country(
    Guid Id,
    string Name,
    string Code,
    Cases Cases);