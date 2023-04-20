using MediatR;
using Telegram.Bot.Types;
using Wanderlust.Application.Departure.Features.GetDepartureCityInfo;
using Wanderlust.Application.Flights.Features.FindVisaFreeZoneFlights.Queries;
using Wanderlust.Web.Interfaces;
using Wanderlust.Web.Models;

namespace Wanderlust.Web.Services;

public class UpdateProcessor : IUpdateProcessor
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateProcessor> _logger;
    private readonly ICallbackQueryDataService _callbackQueryDataService;
    private readonly IReplyService _replyService;

    public UpdateProcessor(
        IMediator mediator,
        ILogger<UpdateProcessor> logger,
        ICallbackQueryDataService callbackQueryDataService,
        IReplyService replyService)
    {
        _mediator = mediator;
        _logger = logger;
        _callbackQueryDataService = callbackQueryDataService;
        _replyService = replyService;
    }

    public async Task ProcessTextMessageAsync(Message message)
    {
        if (string.Equals(message.Text, "/start", StringComparison.OrdinalIgnoreCase))
        {
            await _replyService.SendUsageMessageAsync(message);

            return;
        }

        var query = new GetDepartureCityInfoQuery(message.Text!);
        var result = await _mediator.Send(query);

        if (result is null)
        {
            _logger.LogInformation(
                "Could not find '{City}' city requested by user with username: '{Username}'.",
                message.Text,
                message.From!.Username);

            await _replyService.SendUsageMessageAsync(message);

            return;
        }

        _logger.LogInformation("Requested flights search from '{CityName}'.", result.Name);

        await _replyService.SendSelectRegionMenuAsync(message, result.Code);
    }

    public async Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Message is null)
        {
            _logger.LogError(
                "Callback query message missed. Received from user with username '{Username}'.",
                callbackQuery.From.Username);

            throw new InvalidOperationException("Callback query message missed.");
        }

        var callbackData = _callbackQueryDataService.Parse(callbackQuery);

        switch (callbackData)
        {
            case VisaFreeZoneFlightsMenuData data:
                var query = new FindVisaFreeZoneFlightsQuery(
                    data.DepartureCode,
                    data.Region,
                    data.Date);

                var result = await _mediator.Send(query);

                await _replyService.ReplyWithVisaFreeZoneFlightsAsync(
                    callbackQuery.Message!,
                    result);

                break;

            case BackToSelectRegionMenuData data:
                await _replyService.BackToRequestSelectRegionMenuAsync(
                        callbackQuery.Message, data.DepartureCode);

                break;

            case CloseSelectRegionMenuData:
                await _replyService.CloseSelectRegionMenuAsync(
                    callbackQuery.Message);

                break;

            default:
                throw new InvalidOperationException(
                    $"Hit unhandled callback query data type '{callbackData.GetType().Name}'.");
        }
    }
}