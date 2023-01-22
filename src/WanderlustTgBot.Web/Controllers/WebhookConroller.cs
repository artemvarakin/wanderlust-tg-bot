using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using WanderlustTgBot.Web.Abstractions;

namespace WanderlustTgBot.Web.Controllers;

public class WebhookController : ControllerBase
{
    private readonly IUpdateHandler _updateHandlerService;

    public WebhookController(IUpdateHandler updateHandlerService)
    {
        _updateHandlerService = updateHandlerService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        await _updateHandlerService.HandleAsync(update);
        return Ok();
    }
}