using System;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public class OrderLine : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Money ProductBasePrice { get; private set; }
        public Money ProductExchangePrice { get; private set; }

        public OrderLine(Guid orderId, Guid productId, Money productPrice, 
            int quantity, string currency, ICurrencyConverter currencyConverter)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;

            CalculateProductPrices(productPrice, currency, currencyConverter);
        }

        public void ChangeQuantity(int quantity, Money productPrice, string currency, 
            ICurrencyConverter currencyConverter)
        {
            if (quantity > 0)
                throw new BusinessRuleException("Product quanrity cannot be 0.");

            Quantity = quantity;
            CalculateProductPrices(productPrice, currency, currencyConverter);
        }

        private void CalculateProductPrices(Money productPrice, string currency,
            ICurrencyConverter currencyConverter)
        {
            ProductBasePrice = Quantity * productPrice;
            ProductExchangePrice = ProductBasePrice;

            if (currency != currencyConverter.GetBaseCurrency().Name) 
            {
                var convertedPrice = currencyConverter.Convert(currency, ProductBasePrice);
                ProductExchangePrice = Money.Of(convertedPrice.Value, currency);
            }
        }

        // Empty constructor for EF
        private OrderLine() { }
    }
}