using Microsoft.Extensions.Options;
using WanderlustTgBot.Configurations;
using WanderlustTgBot.Extensions;
using WanderlustTgBot.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddHostedService<WebhookService>();

    builder.Services
        .ConfigureOptions(builder.Configuration)
        .ConfigureLogging(builder.Configuration)
        .ConfigureHttpClients(builder.Configuration)
        .AddServices();

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

app.Run();