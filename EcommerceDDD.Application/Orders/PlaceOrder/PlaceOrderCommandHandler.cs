using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Application.Base;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Carts;
using System.Linq;
using System.Collections.Generic;
using EcommerceDDD.Domain.SharedKernel;

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

        public override async Task<Guid> ExecuteCommand(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            var cartId = CartId.Of(command.CartId);
            var cart = await _unitOfWork.CartRepository.GetCartById(cartId, cancellationToken);
            var customerId = CustomerId.Of(command.CustomerId);
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(customerId, cancellationToken);
            var productsData = new List<CartItemProductData>();

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (cart == null)
                throw new InvalidDataException("Cart not found.");

            var currency = Currency.FromCode(command.Currency);

            var products = await _unitOfWork.ProductRepository
                .GetProductsByIds(cart.Items.Select(i => i.ProductId).ToList());

            if (products == null)
                throw new InvalidDataException("Products couldn't be loaded.");

            foreach (var item in cart.Items)
            {
                var product = products.Where(p => p.Id == item.ProductId).FirstOrDefault();
                productsData.Add(new CartItemProductData(product.Id, product.Price, item.Quantity));
            }

            var order = customer.PlaceOrder(customerId, productsData, currency, _currencyConverter);

            // Cleaning the cart
            cart.Clear();

            await _unitOfWork.CommitAsync();
            return order.Id.Value;
        }
    }
}
