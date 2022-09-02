using Marten;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Quotes.Application.GettingOpenQuote;

public class GetOpenQuoteHandler : QueryHandler<GetOpenQuote, QuoteViewModel>
{
    private readonly IProductsService _productsService;
    private readonly IQuerySession _querySession;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public GetOpenQuoteHandler(
        IProductsService productsService,
        IQuerySession querySession,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _querySession = querySession;
        _productsService = productsService;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public async override Task<QuoteViewModel> Handle(GetOpenQuote query, CancellationToken cancellationToken)
    {
        var projectedQuote = _querySession.Query<QuoteDetails>()
            .FirstOrDefault(q => q.CustomerId == query.CustomerId
            && q.QuoteStatus == QuoteStatus.Open);

        QuoteViewModel viewModel = default!;
        var currency = Currency.OfCode(query.CurrencyCode);

        if (projectedQuote != null)
        {
            viewModel = new QuoteViewModel() with
            {
                QuoteId = projectedQuote.Id,
                CustomerId = projectedQuote.CustomerId,
                CreatedAt = projectedQuote.CreatedAt,
                QuoteStatus = projectedQuote.QuoteStatus.ToString(),
                Items = new List<QuoteItemViewModel>()
            };

            if (projectedQuote.Items != null 
                && projectedQuote.Items.Count > 0)
            {
                var producIds = projectedQuote.Items
                    .Select(pid => pid.ProductId)
                    .ToArray();

                // Obtaining product info from products service
                var products = await _productsService
                    .GetProductByIds(_integrationServicesSettings.ApiGatewayBaseUrl,
                        producIds, query.CurrencyCode);

                var catalogItems = new List<QuoteItemViewModel>();
                foreach (var quoteItem in projectedQuote.Items)
                {
                    var product = products.FirstOrDefault(p => p.Id == quoteItem.ProductId);
                    if (product == null)
                        throw new ApplicationException($"The product {product.Id} is invalid.");

                    catalogItems.Add(new QuoteItemViewModel()
                    {
                        ProductId = product.Id,
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