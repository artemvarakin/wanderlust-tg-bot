namespace Wanderlust.LocationsDbInitializer.Configurations;

public class TravelpayoutsDataApiClientSettings
{
    public const string SectionName = "TravelpayoutsDataApiClient";
    public Uri Url { get; init; } = null!;
    public int RequestTimeout { get; init; } = 30;
}