namespace Wanderlust.TravelpayoutsClients.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}