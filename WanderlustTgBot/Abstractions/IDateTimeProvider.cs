namespace WanderlustTgBot.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}