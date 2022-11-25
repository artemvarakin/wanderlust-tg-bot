namespace WanderlustTgBot.Data.Models;

public class FlightsForMonth
{
    public IEnumerable<Flight> Data { get; init; } = null!;
    public string Currency { get; init; } = null!;
}