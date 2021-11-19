using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Orders;

public class OrderLine : Entity<Guid>
{
    public OrderId OrderId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money ProductBasePrice { get; private set; }
    public Money ProductExchangePrice { get; private set; }

    internal static OrderLine CreateNew(OrderId orderId, ProductId productId, Money productPrice,
        int quantity, Currency currency, ICurrencyConverter currencyConverter)
    {
        return new OrderLine(orderId, productId, productPrice, quantity, currency, currencyConverter);
    }

    private void CalculateProductPrices(Money productPrice, Currency currency,
        ICurrencyConverter currencyConverter)
    {
        ProductBasePrice = Quantity * productPrice;
        var convertedPrice = currencyConverter.Convert(currency, ProductBasePrice);

        if (convertedPrice == null)
            throw new BusinessRuleException("A valid product price must be provided.");

        ProductExchangePrice = Money.Of(convertedPrice.Value, currency.Code);            
    }

    private OrderLine(OrderId orderId, ProductId productId, Money productPrice,
        int quantity, Currency currency, ICurrencyConverter currencyConverter)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;

        CalculateProductPrices(productPrice, currency, currencyConverter);
    }

    // Empty constructor for EF
    private OrderLine() { }
}