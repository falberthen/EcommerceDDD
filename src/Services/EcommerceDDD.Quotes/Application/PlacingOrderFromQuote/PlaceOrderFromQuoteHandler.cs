using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Quotes.Application.PlacingOrderFromQuote;

public class PlaceOrderFromQuoteHandler : CommandHandler<PlaceOrderFromQuote>
{
    private readonly IOrdersService _ordersService;
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public PlaceOrderFromQuoteHandler(
        IOrdersService ordersService,
        IEventStoreRepository<Quote> quoteWriteRepository,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _ordersService = ordersService;
        _quoteWriteRepository = quoteWriteRepository;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(PlaceOrderFromQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStream(command.QuoteId.Value);

        if (quote == null)
            throw new ApplicationException("The quote was not found.");
       
        // Placing order
        await PlaceOrderFromQuote(quote, command.CurrencyCode, cancellationToken);

        // Quote confirmed
        quote.Confirm();

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }

    private async Task PlaceOrderFromQuote(Quote quote, string currencyCode, CancellationToken cancellationToken)
    {                
        var quoteItems = quote.Items.Select(qi =>
            new QuoteItemRequest(
                qi.ProductId.Value,
                qi.Quantity)
            ).ToList();

        var request = new PlaceOrderRequest(
                quote.Id.Value,
                quote.CustomerId.Value,
                quoteItems,
                currencyCode);

        var response = await _ordersService
            .RequestPlaceOrder(
                _integrationServicesSettings.ApiGatewayBaseUrl, 
                request);

        if (!response.Success)
            throw new ApplicationException("Error placing the order.");
    }
}