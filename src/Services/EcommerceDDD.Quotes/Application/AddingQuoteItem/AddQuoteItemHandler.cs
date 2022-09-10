using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Quotes.Application.AddingQuoteItem;

public class AddQuoteItemHandler : CommandHandler<AddQuoteItem>
{
    private readonly IProductsService _productsService;
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public AddQuoteItemHandler(
        IProductsService productsService,
        IEventStoreRepository<Quote> quoteWriteRepository,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _productsService = productsService;
        _quoteWriteRepository = quoteWriteRepository;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(AddQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
            .FetchStream(command.QuoteId.Value);

        if (quote == null)
            throw new ApplicationException("The quote was not found.");

        // Checking if product is valid
        await ValidateProduct(command, cancellationToken);

        var quotetemData = new QuoteItemData(
            command.QuoteId,
            command.ProductId,
            command.Quantity);

        quote.AddQuoteItem(quotetemData);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }

    private async Task ValidateProduct(AddQuoteItem command, CancellationToken cancellationToken)
    {
        var productIds = new Guid[] { command.ProductId.Value };
        var products = await _productsService
            .GetProductByIds(_integrationServicesSettings.ApiGatewayBaseUrl,
                productIds, command.CurrencyCode);

        if (products == null || productIds.Count() == 0)
            throw new ApplicationException($"The product {command.ProductId} is invalid.");
    }
}