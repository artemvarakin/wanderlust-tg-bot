using WanderlustTgBot.Abstractions;

namespace WanderlustTgBot.Services;

// todo: consider to split to services
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
        var departureCode = await _autocompleteApiClient
            .GetDepartureCodeByNameAsync(message.Text!);

        if (departureCode is null)
        {
            await _replyService.ReplyWithUsageMessage(message);
            return;
        }

        await _replyService.RequestSearchTypeAsync(message, departureCode);
    }

    public async Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        // should not happen but tg library
        // states that Message is nullable
        if (callbackQuery.Message is null)
        {
            _logger.LogError(
                "Callback query message missed. Received from {user}.",
                callbackQuery.From);
            throw new InvalidOperationException("Callback query message missed.");
        }

        var searchRequest = _callbackDataService.ParseFlightsSearchRequest(callbackQuery);

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