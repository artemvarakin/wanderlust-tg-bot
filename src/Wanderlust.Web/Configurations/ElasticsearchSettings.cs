namespace Wanderlust.Web.Configurations;

public record ElasticsearchSettings
{
    public const string SectionName = "Elasticsearch";
    public Uri Uri { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ApplicationName { get; init; } = null!;
}