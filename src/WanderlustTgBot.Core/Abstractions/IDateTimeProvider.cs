namespace WanderlustTgBot.Core.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}