using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base.Commands;
using EcommerceDDD.Domain;
using System.Linq;
using System.Collections.Generic;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.CurrencyExchange;
using static EcommerceDDD.Domain.Customers.Orders.Basket;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderCommandHandler : CommandHandler<PlaceOrderCommand, CommandHandlerResult>
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

        public override async Task<Guid> RunCommand(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(command.CustomerId);
            Guid orderId = new Guid();

            if(customer != null)
            {
                var productIds = command.Products.Select(p => p.Id).ToList();
                List<Product> products = await _unitOfWork.ProductRepository.GetProductsByIds(productIds);

                if (products.Count > 0)
                {
                    Basket basket = new Basket(command.Currency);
                    foreach (var product in products)
                    {
                        var quantity = command.Products.FirstOrDefault(p => p.Id == product.Id).Quantity;
                        basket.AddProduct(product.Id, product.Price, quantity);
                    }

                    orderId = customer.PlaceOrder(basket, _currencyConverter);

                    await _unitOfWork.CustomerRepository.AddCustomerOrders(customer);
                    await _unitOfWork.CommitAsync();
                }
            }

            return orderId;
        }
    }
}
