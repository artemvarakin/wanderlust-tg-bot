using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wanderlust.Application.Common.Behaviors;
using Wanderlust.Application.Common.Interfaces;
using Wanderlust.Application.Common.Services;

namespace Wanderlust.Application.Common.Extensions;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        return services
            .AddMediatR(Assembly.GetCallingAssembly())
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    public static IServiceCollection AddDataMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        return services
            .AddSingleton(config)
            .AddScoped<IMapper, ServiceMapper>();
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IFlightsProvider, FlightsProvider>()
            .AddScoped<IDatesProvider, DatesProvider>()
            .AddScoped<IDepartureBoardsProvider, DepartureBoardsProvider>();
    }
}