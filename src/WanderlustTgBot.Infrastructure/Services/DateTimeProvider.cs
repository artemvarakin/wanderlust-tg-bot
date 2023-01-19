using WanderlustTgBot.Core.Abstractions;

namespace WanderlustTgBot.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}