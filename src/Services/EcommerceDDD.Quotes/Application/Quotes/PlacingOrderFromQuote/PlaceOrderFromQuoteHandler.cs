using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Infrastructure.Integration;

namespace EcommerceDDD.Quotes.Application.Quotes.PlacingOrderFromQuote;

public class PlaceOrderFromQuoteHandler : ICommandHandler<PlaceOrderFromQuote>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public PlaceOrderFromQuoteHandler(
        IIntegrationHttpService integrationHttpService,
        IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _integrationHttpService = integrationHttpService;
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task<Unit> Handle(PlaceOrderFromQuote command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException("Quote not found.");

        // Placing order
        await PlaceOrderFromQuote(quote, command.Currency.Code);

        // Quote confirmed
        quote.Confirm();

        await _quoteWriteRepository
            .AppendEventsAsync(quote);

        return Unit.Value;
    }

    private async Task PlaceOrderFromQuote(Quote quote, string currencyCode)
    {
        // Pre-generates the order id
        var orderId = Guid.NewGuid();

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

        var response = await _integrationHttpService.PostAsync(
            $"api/orders/{orderId}",
            request);

        if (response is null)
            throw new ApplicationLogicException($"An error occurred placing order {orderId}.");

        if (!response.Success)
            throw new ApplicationLogicException(response.Message);
    }
}