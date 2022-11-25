using WanderlustTgBot.Abstractions;

namespace WanderlustTgBot.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}