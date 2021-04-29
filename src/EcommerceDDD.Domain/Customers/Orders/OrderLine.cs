using System;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public class OrderLine : Entity<Guid>
    {
        public OrderId OrderId { get; private set; }
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Money ProductBasePrice { get; private set; }
        public Money ProductExchangePrice { get; private set; }

        public OrderLine(Guid id, OrderId orderId, ProductId productId, Money productPrice,
            int quantity, Currency currency, ICurrencyConverter currencyConverter)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            
            CalculateProductPrices(productPrice, currency, currencyConverter);
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

        // Empty constructor for EF
        private OrderLine() { }
    }
}