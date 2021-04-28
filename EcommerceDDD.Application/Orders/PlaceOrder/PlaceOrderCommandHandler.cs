using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Shared;
using BuildingBlocks.CQRS.CommandHandling;

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
            var cart = await _unitOfWork.CartRepository.GetById(command.CartId, cancellationToken);
            var customer = await _unitOfWork.CustomerRepository.GetById(command.CustomerId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (cart == null)
                throw new InvalidDataException("Cart not found.");

            var currency = Currency.FromCode(command.Currency);
            var order = Order.PlaceOrder(Guid.NewGuid(), cart, currency, _currencyConverter);

            // Cleaning the cart
            cart.Clear();

            await _unitOfWork.OrderRepository.Add(order, cancellationToken);
            await _unitOfWork.CommitAsync();                                

            return order.Id;
        }
    }
}
