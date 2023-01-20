using System.Globalization;
using Microsoft.Extensions.Options;
using WanderlustTgBot.Extensions;
using WanderlustTgBot.Infrastructure.Configurations;
using WanderlustTgBot.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .ConfigureOptions(builder.Configuration)
        .ConfigureLogging(builder.Configuration)
        .ConfigureHttpClients(builder.Configuration)
        .AddRedisCache(builder.Configuration);

    builder.Services
        .AddHostedService<WebhookService>()
        .AddPresentationServices()
        .AddInfrastructureServices();

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
                .GetRequiredService<IOptions<TelegramBotClientSettings>>().Value;

            builder.MapControllerRoute(
                name: "webhook",
                pattern: $"/{botSettings.Token}",
                new { controller = "webhook", action = "post" });
        });
}

// localization setup
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");

app.Run();