using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Quotes.Application.Products;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Application.Quotes.AddingQuoteItem;

public class AddQuoteItemHandler : ICommandHandler<AddQuoteItem>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;

    public AddQuoteItemHandler(
        IIntegrationHttpService integrationHttpService,
        IEventStoreRepository<Quote> quoteWriteRepository)
    {
        _integrationHttpService = integrationHttpService;
        _quoteWriteRepository = quoteWriteRepository;
    }

    public async Task Handle(AddQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStreamAsync(command.QuoteId.Value)
            ?? throw new RecordNotFoundException("The quote was not found.");

        // checks if product exists in the catalog
        var response = await _integrationHttpService.FilterAsync<List<ProductViewModel>>(
            "api/products",
            new GetProductsRequest(command.Currency.Code, new Guid[] { command.ProductId.Value }));

        if (response is null || !response.Success)
            throw new ApplicationLogicException($"The was a problem when getting products.");

        if (response.Data is null)
            throw new RecordNotFoundException($"The product {command.ProductId.Value} is invalid.");

        var quotetemData = new QuoteItemData(
            command.QuoteId,
            command.ProductId,
            command.Quantity);

        quote.AddItem(quotetemData);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}