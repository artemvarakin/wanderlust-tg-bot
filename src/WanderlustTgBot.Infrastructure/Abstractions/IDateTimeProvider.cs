namespace WanderlustTgBot.Infrastructure.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}