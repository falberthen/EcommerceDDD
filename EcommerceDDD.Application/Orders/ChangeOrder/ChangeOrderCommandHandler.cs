using EcommerceDDD.Application.Base.Commands;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EcommerceDDD.Domain.Customers.Orders.Basket;

namespace EcommerceDDD.Application.Orders.ChangeOrder
{
    public class ChangeOrderCommandHandler : CommandHandler<ChangeOrderCommand, CommandHandlerResult>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _currencyConverter;

        public ChangeOrderCommandHandler(
            IEcommerceUnitOfWork unitOfWork,
            ICurrencyConverter converter)
        {
            _unitOfWork = unitOfWork;
            _currencyConverter = converter;
        }

        public override async Task<Guid> RunCommand(ChangeOrderCommand command, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(command.CustomerId);

            if (customer != null)
            {
                var productIds = command.Products.Select(p => p.Id).ToList();
                List<Product> products = await _unitOfWork.ProductRepository.GetProductsByIds(productIds);

                if (products.Count > 0)
                {
                    Basket basket = new Basket(command.Currency);
                    foreach (var product in products)
                    {
                        var quantity = command.Products.Where(p => p.Id == product.Id).FirstOrDefault().Quantity;
                        basket.AddProduct(product.Id, product.Price, quantity);
                    }

                    customer.ChangeOrder(basket, command.OrderId, _currencyConverter);
                    await _unitOfWork.CustomerRepository.ChangeCustomerOrder(customer, command.OrderId);
                    await _unitOfWork.CommitAsync();
                }
            }

            return command.OrderId;
        }
    }
}
