using System;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.Orders
{
    public class OrderLine : Entity<Guid>
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Money ProductBasePrice { get; private set; }
        public Money ProductExchangePrice { get; private set; }

        public OrderLine(Guid id, Guid orderId, Guid productId, Money productPrice, 
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