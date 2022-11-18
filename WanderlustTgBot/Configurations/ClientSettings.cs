namespace WanderlustTgBot.Configurations;

public class ClientSettings
{
    public const string SectionName = "ClientSettings";
    public Uri AutocompleteApiUrl { get; init; } = null!;
}