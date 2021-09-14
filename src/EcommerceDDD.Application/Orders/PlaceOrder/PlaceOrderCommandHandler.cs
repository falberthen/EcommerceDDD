using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Application.Base;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Quotes;
using System.Linq;
using System.Collections.Generic;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Domain.Orders;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderCommandHandler : CommandHandler<PlaceOrderCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _currencyConverter;        

        public PlaceOrderCommandHandler(
            IEcommerceUnitOfWork unitOfWork,
            ICurrencyConverter converter)
        {
            _unitOfWork = unitOfWork;
            _currencyConverter = converter;
        }

        public override async Task<Guid> ExecuteCommand(PlaceOrderCommand command, 
            CancellationToken cancellationToken)
        {
            var customerId = CustomerId.Of(command.CustomerId);
            var productsData = new List<QuoteItemProductData>();
            var quoteId = QuoteId.Of(command.QuoteId);
            var quote = await _unitOfWork.Quotes
                .GetById(quoteId, cancellationToken);            
            var customer = await _unitOfWork.Customers
                .GetById(customerId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (quote == null)
                throw new InvalidDataException("Quote not found.");

            var currency = Currency.FromCode(command.Currency);

            var products = await _unitOfWork.Products
                .GetByIds(quote.Items.Select(i => i.ProductId).ToList());

            if (products == null)
                throw new InvalidDataException("Products couldn't be loaded.");

            foreach (var item in quote.Items)
            {
                var product = products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefault();

                productsData.Add(
                    new QuoteItemProductData(product.Id, product.Price, item.Quantity)
                );
            }

            var order = Order.PlaceOrder(customerId, quoteId, productsData, currency, _currencyConverter);
            await _unitOfWork.Orders.Add(order);
            await _unitOfWork.CommitAsync();
            return order.Id.Value;
        }
    }
}
