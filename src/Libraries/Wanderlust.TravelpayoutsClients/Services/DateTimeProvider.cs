using Wanderlust.TravelpayoutsClients.Interfaces;

namespace Wanderlust.TravelpayoutsClients.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}