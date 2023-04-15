namespace Wanderlust.Infrastructure.Configurations;

public class LocationsDatabaseSettings
{
    public const string SectionName = "LocationsDatabase";
    public string DatabaseName { get; init; } = null!;
    public string CitiesCollectionName { get; init; } = null!;
    public string VisaFreeDirectionsCollectionName { get; init; } = null!;
}