using System.Globalization;
using Microsoft.Extensions.Options;
using WanderlustTgBot.Core.Configurations;
using WanderlustTgBot.Extensions;
using WanderlustTgBot.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .ConfigureLogging(builder.Configuration)
        .ConfigureOptions(builder.Configuration)
        .ConfigureTelegramBotHttpClient(builder.Configuration);

    builder.Services
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