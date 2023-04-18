using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Wanderlust.Web.Interfaces;

namespace Wanderlust.Web.Controllers;

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