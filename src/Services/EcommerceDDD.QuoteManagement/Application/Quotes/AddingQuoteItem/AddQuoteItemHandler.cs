namespace EcommerceDDD.QuoteManagement.Application.Quotes.AddingQuoteItem;

public class AddQuoteItemHandler(
    IEventStoreRepository<Quote> quoteWriteRepository,
    IProductMapper productMapper
) : ICommandHandler<AddQuoteItem>
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
	private readonly IProductMapper _productMapper = productMapper;

	public async Task HandleAsync(AddQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
			.FetchStreamAsync(command.QuoteId.Value, cancellationToken: cancellationToken)
            ?? throw new RecordNotFoundException($"The quote {command.QuoteId} was not found.");

        // Getting product data from catalog
        var currency = Currency.OfCode(quote.Currency.Code);
        var productData = await _productMapper
			.MapProductFromCatalogAsync([command.ProductId], currency, cancellationToken)
            ?? throw new ApplicationLogicException($"Product {command.ProductId} is invalid.");
        var product = productData.FirstOrDefault()
            ?? throw new ApplicationLogicException($"Product {command.ProductId} is invalid.");

		decimal productPrice = Convert.ToDecimal(product.Price!.Value);
		var quotetemData = new QuoteItemData(
			quote.Id,
			command.ProductId,
			product.Name!,
			Money.Of(productPrice, currency.Code),
			command.Quantity);

		quote.AddItem(quotetemData);

		await _quoteWriteRepository
			.AppendEventsAsync(quote, cancellationToken);
    }
}