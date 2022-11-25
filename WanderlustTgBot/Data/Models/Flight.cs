using System.Text.Json.Serialization;

namespace WanderlustTgBot.Data.Models;

public class Flight
{
    public string Origin { get; init; } = null!;
    public string Destination { get; init; } = null!;
    public int Price { get; init; }
    [JsonPropertyName("departure_at")]
    public DateTime DepartureAt { get; init; }
    public int Transfers { get; init; }
    public string Link { get; init; } = null!;
}