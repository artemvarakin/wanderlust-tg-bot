using Wanderlust.Infrastructure.Interfaces;

namespace Wanderlust.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}