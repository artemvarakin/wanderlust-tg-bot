using Telegram.Bot.Types;
using WanderlustTgBot.Core.Abstractions;
using WanderlustTgBot.Web.Abstractions;

namespace WanderlustTgBot.Web.Services;

public class UpdateProcessor : IUpdateProcessor
{
    private readonly IAutocompleteApiClient _autocompleteApiClient;
    private readonly ICallbackDataService _callbackDataService;
    private readonly IFlightsProvider _flightsProvider;
    private readonly IReplyService _replyService;
    private readonly ILogger<UpdateProcessor> _logger;

    public UpdateProcessor(
        IAutocompleteApiClient autocompleteApiClient,
        ICallbackDataService callbackDataService,
        IFlightsProvider flightsProvider,
        IReplyService replyService,
        ILogger<UpdateProcessor> logger)
    {
        _autocompleteApiClient = autocompleteApiClient;
        _callbackDataService = callbackDataService;
        _flightsProvider = flightsProvider;
        _replyService = replyService;
        _logger = logger;
    }

    public async Task ProcessTextMessageAsync(Message message)
    {
        if (message.Text is null)
        {
            _logger.LogWarning(
                "An empty message received from user: {user}",
                message.From);

            return;
        }

        var departureCode = await _autocompleteApiClient
            .GetDepartureCodeByNameAsync(message.Text);

        if (departureCode is null)
        {
            await _replyService.ReplyWithUsageMessage(message);
            _logger.LogInformation(
                "Could not find '{city}' city requested by user: {user}",
                message.Text,
                message.From);

            return;
        }

        await _replyService.RequestSearchTypeAsync(message, departureCode);
    }

    public async Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Message is null)
        {
            _logger.LogError(
                "Callback query message missed. Received from {user}.",
                callbackQuery.From);
            throw new InvalidOperationException("Callback query message missed.");
        }

        var searchRequest = _callbackDataService
            .ParseFlightsSearchRequest(callbackQuery);

        switch (searchRequest.SearchType)
        {
            case FlightsSearchType.VisaFreeZone:

                var departureBoard = await _flightsProvider
                    .GetVisaFreeZoneFlights(searchRequest);

                await _replyService.ReplyWithVisaFreeZoneFlightsAsync(
                    callbackQuery.Message,
                    departureBoard,
                    searchRequest.DepartureCode);

                return;

            case FlightsSearchType.SpecificDirection:
                return;

            default:
                // todo: handle
                return;
        }
    }
}