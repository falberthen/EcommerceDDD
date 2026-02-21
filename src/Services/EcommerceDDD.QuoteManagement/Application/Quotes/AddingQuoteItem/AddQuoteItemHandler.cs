namespace EcommerceDDD.QuoteManagement.Application.Quotes.AddingQuoteItem;

public class AddQuoteItemHandler(
    IEventStoreRepository<Quote> quoteWriteRepository,
    IProductMapper productMapper
) : ICommandHandler<AddQuoteItem>
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
	private readonly IProductMapper _productMapper = productMapper;

	public async Task<Result> HandleAsync(AddQuoteItem command, CancellationToken cancellationToken)
    {
        var quote = await _quoteWriteRepository
			.FetchStreamAsync(command.QuoteId.Value, cancellationToken: cancellationToken);

        if (quote is null)
            return Result.Fail($"The quote {command.QuoteId} was not found.");

        var currency = Currency.OfCode(quote.Currency.Code);
        var productDataResult = await _productMapper
			.MapProductFromCatalogAsync([command.ProductId], currency, cancellationToken);

        if (productDataResult.IsFailed)
            return Result.Fail(productDataResult.Errors);

        var product = productDataResult.Value!.FirstOrDefault();
        if (product is null)
            return Result.Fail($"Product {command.ProductId} is invalid.");

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

        return Result.Ok();
    }
}
