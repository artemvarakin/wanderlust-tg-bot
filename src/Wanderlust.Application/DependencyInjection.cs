using Microsoft.Extensions.DependencyInjection;
using Wanderlust.Application.Common.Extensions;

namespace Wanderlust.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediatR()
            .AddDataMappings()
            .AddFluentValidation()
            .AddServices();
    }
}