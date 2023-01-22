namespace WanderlustTgBot.Core.Configurations;

public class AutocompleteApiClientSettings
{
    public const string SectionName = "AutocompleteApiClient";
    public Uri Url { get; init; } = null!;
    public int RequestTimeout { get; init; } = 10;
}