namespace WanderlustTgBot.Core.Configurations;

public class AviasalesApiClientSettings
{
    public const string SectionName = "AviasalesApiClient";
    public Uri Url { get; init; } = null!;
    public int RequestTimeout { get; init; } = 10;
    public string ApiToken { get; init; } = null!;
}