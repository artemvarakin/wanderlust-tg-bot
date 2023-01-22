namespace WanderlustTgBot.Core.Configurations;

public class TgBotClientSettings
{
    public const string SectionName = "TelegramBot";
    public string Token { get; init; } = null!;
    public Uri HostName { get; init; } = null!;
}