namespace Wanderlust.Infrastructure.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}