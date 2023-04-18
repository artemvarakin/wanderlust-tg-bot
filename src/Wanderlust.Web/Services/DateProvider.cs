using Wanderlust.Web.Interfaces;

namespace Wanderlust.Web.Services;

public class DateProvider : IDateProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}