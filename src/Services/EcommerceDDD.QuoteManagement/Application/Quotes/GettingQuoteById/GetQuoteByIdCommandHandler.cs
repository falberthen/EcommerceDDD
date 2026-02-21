namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingQuoteById;

public class GetQuoteByIdCommandHandler(
	IQuerySession querySession,
	IProductMapper productMapper
) : IQueryHandler<GetQuoteById, QuoteViewModel>
{
	private readonly IQuerySession _querySession = querySession;
	private readonly IProductMapper _productMapper = productMapper;

	public async Task<Result<QuoteViewModel>> HandleAsync(GetQuoteById query, CancellationToken cancellationToken)
	{
		QuoteDetails? quoteDetails = default;
		var queryExpression = _querySession.Query<QuoteDetails>();
		quoteDetails = await queryExpression
			.FirstOrDefaultAsync(q =>
				q.Id == query.QuoteId.Value, token: cancellationToken);

		QuoteViewModel viewModel = default!;
		if (quoteDetails is not null)
		{
			viewModel = new QuoteViewModel() with
			{
				QuoteId = quoteDetails.Id,
				CustomerId = quoteDetails.CustomerId,
				CreatedAt = quoteDetails.CreatedAt,
				QuoteStatus = quoteDetails.QuoteStatus.ToString(),
				CurrencyCode = quoteDetails.CurrencyCode,
				Items = new List<QuoteItemViewModel>()
			};

			if (quoteDetails.Items is not null
				&& quoteDetails.Items.Count > 0)
			{
				var producIds = quoteDetails.Items
					.Select(pid => ProductId.Of(pid.ProductId))
					.ToList();

				Currency currency = Currency.OfCode(quoteDetails.CurrencyCode);

				var productsDataResult = await _productMapper
					.MapProductFromCatalogAsync(producIds, currency, cancellationToken);

				if (productsDataResult.IsFailed)
					return Result.Fail<QuoteViewModel>(productsDataResult.Errors);

				var productsData = productsDataResult.Value!;

				var catalogItems = new List<QuoteItemViewModel>();
				foreach (var quoteItem in quoteDetails.Items)
				{
					var product = productsData.FirstOrDefault(p =>
						p.ProductId == quoteItem.ProductId);

					if (product is null)
						return Result.Fail<QuoteViewModel>(
							$"The product {quoteItem.ProductId} is invalid.");

					catalogItems.Add(new QuoteItemViewModel()
					{
						ProductId = product.ProductId!.Value,
						ProductName = product.Name!,
						UnitPrice = Convert.ToDecimal(product.Price!.Value),
						CurrencySymbol = currency.Symbol,
						Quantity = quoteItem.Quantity
					});
				}

				viewModel = viewModel with { Items = catalogItems };
				viewModel.CurrencySymbol = currency.Symbol;
			}
		}

		return Result.Ok(viewModel);
	}
}
