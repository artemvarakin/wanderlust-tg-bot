using System.Text.Json;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Wanderlust.Core.Domain;
using Wanderlust.LocationsDbInitializer.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class VisaFreeDirectionsProvider : IVisaFreeDirectionsProvider
{
    private readonly ILogger<VisaFreeDirectionsProvider> _logger;
    private readonly IVisaFreeDirectionsListReader _reader;
    private readonly IMapper _mapper;

    public VisaFreeDirectionsProvider(
        ILogger<VisaFreeDirectionsProvider> logger,
        IVisaFreeDirectionsListReader reader,
        IMapper mapper)
    {
        _logger = logger;
        _reader = reader;
        _mapper = mapper;
    }

    public IEnumerable<VisaFreeDirection> GetVisaFreeDirections(
        IEnumerable<Country> countries)
    {
        _logger.LogInformation("Started reading visa free directions list.");
        using var stream = _reader.Read();
        var data = JsonSerializer.Deserialize<JsonElement>(stream);

        var idx = countries.ToDictionary(c => c.Name);
        var result = new List<VisaFreeDirection>();

        try
        {
            foreach (var prop in data.EnumerateObject())
            {
                var region = Enum.Parse<GeoRegion>(prop.Name, ignoreCase: true);
                foreach (var entry in prop.Value.EnumerateArray())
                {
                    var countryName = entry.ToString();
                    if (idx.TryGetValue(countryName, out var country))
                    {
                        result.Add(_mapper.Map<VisaFreeDirection>((country, region)));
                    }
                    else
                    {
                        _logger.LogWarning(
                            "'{Name}' does not match any country name received from Travelpayouts. " +
                            "Will not be added to database.",
                            countryName);
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to parse data.", e);
        }

        return result;
    }
}
