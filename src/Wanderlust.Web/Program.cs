using System.Globalization;
using Microsoft.Extensions.Options;
using Wanderlust.Application;
using Wanderlust.Infrastructure;
using Wanderlust.Web.Configurations;
using Wanderlust.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .ConfigureOptions(builder.Configuration)
        .ConfigureLogging(builder.Configuration, builder.Environment.EnvironmentName)
        .ConfigureTelegramBotHttpClient(builder.Configuration);

    builder.Services
        .AddApplicationServices()
        .AddPresentationServices()
        .AddInfrastructureServices(builder.Configuration);

    builder.Services
        .AddControllers()
        .AddNewtonsoftJson();
}

var app = builder.Build();
{
    app
        .UseHttpsRedirection()
        .UseRouting()
        .UseEndpoints(builder =>
        {
            var botSettings = app.Services
                .GetRequiredService<IOptions<TgBotClientSettings>>().Value;

            builder.MapControllerRoute(
                name: "webhook",
                pattern: $"/{botSettings.Token}",
                new { controller = "webhook", action = "post" });
        });
}

// localization setup
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");

app.Run();