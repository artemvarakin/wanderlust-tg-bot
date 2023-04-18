using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wanderlust.LocationsDbInitializer.Extensions;
using Wanderlust.LocationsDbInitializer.Interfaces;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services
            .ConfigureLogging(ctx.Configuration)
            .ConfigureTravelpayoutsDataApiClient(ctx.Configuration)
            .ConfigureLocationsDbClient(ctx.Configuration)
            .AddDataMappings()
            .AddServices();
    })
    .Build();

using var scope = host.Services.CreateScope();
var executor = scope.ServiceProvider.GetRequiredService<IExecutionService>();

var cancellationToken = new CancellationTokenSource(
    TimeSpan.FromMinutes(1)).Token;

await executor.DoWorkAsync(cancellationToken);