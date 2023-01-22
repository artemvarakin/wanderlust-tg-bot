using Microsoft.Extensions.DependencyInjection;
using WanderlustTgBot.Infrastructure.Extensions;

namespace WanderlustTgBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        return services
            .AddHttpClients()
            .AddRedisCache();
    }
}