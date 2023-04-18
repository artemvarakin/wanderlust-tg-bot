using Microsoft.Extensions.Configuration;
using Wanderlust.LocationsDbInitializer.Interfaces;

namespace Wanderlust.LocationsDbInitializer.Services;

public class VisaFreeDirectionsListReader : IVisaFreeDirectionsListReader
{
    private readonly IConfiguration _configuration;

    public VisaFreeDirectionsListReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public FileStream Read()
    {
        var filePath = _configuration.GetValue<string>("VisaFreeDirectionsListPath");
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException(
                "Visa free directions list path must be specified in config file.");
        }

        return File.OpenRead(filePath);
    }
}