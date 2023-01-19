namespace WanderlustTgBot.Infrastructure.Configurations;

public class TelegramBotClientSettings
{
    public const string SectionName = "TelegramBot";
    public string Token { get; init; } = null!;
    public string HostName { get; init; } = null!;
}