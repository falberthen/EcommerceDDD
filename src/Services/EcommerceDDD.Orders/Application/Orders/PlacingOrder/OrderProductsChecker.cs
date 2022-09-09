using EcommerceDDD.Orders.Domain;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public class OrderProductsChecker : IOrderProductsChecker
{
    private readonly IProductsService _productsService;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public OrderProductsChecker(
        IProductsService productsService,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _productsService = productsService;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public async Task CheckFromQuote(ConfirmedQuote confirmedQuote)
    {
        var producIds = confirmedQuote.Items
            .Select(pid => pid.ProductId.Value)
            .ToArray();

        var currency = confirmedQuote.Currency;
        var products = await _productsService
            .GetProductByIds(_integrationServicesSettings.ApiGatewayBaseUrl, 
                producIds, currency.Code);

        foreach (var quoteItem in confirmedQuote.Items)
        {
            var product = products.FirstOrDefault(p => p.ProductId == quoteItem.ProductId.Value);
            if (product == null)
                throw new ApplicationException($"The product {quoteItem.ProductId.Value} is invalid.");

            if (string.IsNullOrEmpty(product.Name))
                throw new ApplicationException($"The product {product.ProductId} has no name.");

            // Sets the latest product name and price as a projected snapshot
            quoteItem.SetName(product.Name);
            quoteItem.SetPrice(Money.Of(product.Price, currency.Code));
        }
    }
}
