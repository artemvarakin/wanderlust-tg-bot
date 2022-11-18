using System.Text.Json.Serialization;

namespace WanderlustTgBot.Data.Models;

public class DeparturePoint
{
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    [JsonPropertyName("country_code")]
    public string CountryCode { get; init; } = null!;
    [JsonPropertyName("country_name")]
    public string CountryName { get; init; } = null!;

}