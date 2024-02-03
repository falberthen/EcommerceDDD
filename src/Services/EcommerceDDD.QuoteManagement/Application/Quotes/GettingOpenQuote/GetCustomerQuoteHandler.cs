namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingOpenQuote;

public class GetCustomerQuoteHandler : IQueryHandler<GetCustomerQuote, QuoteViewModel>
{
    private readonly IQuerySession _querySession;
    private readonly IProductMapper _productMapper;

    public GetCustomerQuoteHandler(
        IQuerySession querySession,
        IProductMapper productMapper)
    {       
        _querySession = querySession;
        _productMapper = productMapper;
    }

    public async Task<QuoteViewModel> Handle(GetCustomerQuote query, CancellationToken cancellationToken)
    {
        QuoteDetails? quoteDetails = default;
        var queryExpression = _querySession.Query<QuoteDetails>();

        if (query.QuoteId is not null)
        {
            quoteDetails = queryExpression
                .FirstOrDefault(q =>
                    q.Id == query.QuoteId.Value);
        }
        else
        {
            quoteDetails = queryExpression
                .FirstOrDefault(q => 
                    q.CustomerId == query.CustomerId.Value && q.QuoteStatus == QuoteStatus.Open);
        }
        
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