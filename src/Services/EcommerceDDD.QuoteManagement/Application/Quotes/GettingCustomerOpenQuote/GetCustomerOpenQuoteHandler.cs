namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingCustomerOpenQuote;

public class GetCustomerOpenQuoteHandler(
	IUserInfoRequester userInfoRequester,
	IQuerySession querySession,
    IProductMapper productMapper) : IQueryHandler<GetCustomerOpenQuote, QuoteViewModel>
{
    private readonly IQuerySession _querySession = querySession;
    private readonly IProductMapper _productMapper = productMapper;
	private IUserInfoRequester _userInfoRequester { get; set; } = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<QuoteViewModel> Handle(GetCustomerOpenQuote query, CancellationToken cancellationToken)
    {
		CustomerId customerId = default!;
		UserInfo? userInfo = await _userInfoRequester
			.RequestUserInfoAsync();

		customerId = CustomerId.Of(userInfo!.CustomerId);		
		
		QuoteDetails? quoteDetails = default;
        var queryExpression = _querySession.Query<QuoteDetails>();

		// Customer's open quote
		quoteDetails = queryExpression
			.FirstOrDefault(q =>
				q.CustomerId == customerId.Value
				&& q.QuoteStatus == QuoteStatus.Open);

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

                // Getting product data from catalog
                var productsData = await _productMapper
                    .MapProductFromCatalogAsync(producIds, currency)
                    ?? throw new RecordNotFoundException($"The was no data for the provided products.");

                var catalogItems = new List<QuoteItemViewModel>();
                foreach (var quoteItem in quoteDetails.Items)
                {
                    var product = productsData.FirstOrDefault(p => 
                        p.ProductId == quoteItem.ProductId)
                        ?? throw new ApplicationLogicException(
                            $"The product {quoteItem.ProductId} is invalid.");

                    catalogItems.Add(new QuoteItemViewModel()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        CurrencySymbol = currency.Symbol,                        
                        Quantity = quoteItem.Quantity
                    });
                }

                viewModel = viewModel with { Items = catalogItems };
                viewModel.CurrencySymbol = currency.Symbol;
            }
        }

        return viewModel;
    }
}