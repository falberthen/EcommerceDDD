using Marten;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Quotes.Application.Products;
using EcommerceDDD.Quotes.Infrastructure.Projections;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Application.Quotes.GettingOpenQuote;

public class GetOpenQuoteHandler : IQueryHandler<GetOpenQuote, QuoteViewModel>
{
    private readonly IQuerySession _querySession;
    private readonly IIntegrationHttpService _integrationHttpService;

    public GetOpenQuoteHandler(
        IQuerySession querySession,
        IIntegrationHttpService integrationHttpService)
    {       
        _querySession = querySession;
        _integrationHttpService = integrationHttpService;
    }

    public async Task<QuoteViewModel> Handle(GetOpenQuote query, CancellationToken cancellationToken)
    {
        var projectedQuote = _querySession.Query<QuoteDetails>()
            .FirstOrDefault(q => q.CustomerId == query.CustomerId.Value
            && q.QuoteStatus == QuoteStatus.Open);

        QuoteViewModel viewModel = default!;
        var currency = Currency.OfCode(query.CurrencyCode);

        if (projectedQuote is not null)
        {
            viewModel = new QuoteViewModel() with
            {
                QuoteId = projectedQuote.Id,
                CustomerId = projectedQuote.CustomerId,
                CreatedAt = projectedQuote.CreatedAt,
                QuoteStatus = projectedQuote.QuoteStatus.ToString(),
                Items = new List<QuoteItemViewModel>()
            };

            if (projectedQuote.Items is not null
                && projectedQuote.Items.Count > 0)
            {
                var producIds = projectedQuote.Items
                    .Select(pid => pid.ProductId)
                    .ToArray();

                // Obtaining product info from products service                
                var response = await _integrationHttpService.FilterAsync<List<ProductViewModel>>(
                    "api/products",
                    new GetProductsRequest(query.CurrencyCode, producIds));

                if (response is null || !response.Success)
                    throw new ApplicationLogicException($"The was a problem when getting products.");

                var products = response.Data
                    ?? throw new RecordNotFoundException($"The was no data for the provided products.");

                var catalogItems = new List<QuoteItemViewModel>();
                foreach (var quoteItem in projectedQuote.Items)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == quoteItem.ProductId);
                    if (product is null)
                        throw new ApplicationLogicException($"The product {quoteItem.ProductId} is invalid.");

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