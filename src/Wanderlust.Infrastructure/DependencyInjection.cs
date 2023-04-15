using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wanderlust.Infrastructure.Extensions;

namespace Wanderlust.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .ConfigureOptions(configuration)
            .ConfigureLocationsDbClient(configuration)
            .ConfigureDepartureBoardsCache(configuration)
            .AddHttpClients()
            .AddServices();

        return services;
    }
}