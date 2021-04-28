using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Services;
using BuildingBlocks.CQRS.CommandHandling;

namespace EcommerceDDD.Application.Carts.CreateCart
{
    public class SaveCartCommandHandler : CommandHandler<SaveCartCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _currencyConverter;

        public SaveCartCommandHandler(
            IEcommerceUnitOfWork unitOfWork,
            ICurrencyConverter converter)
        {
            _unitOfWork = unitOfWork;
            _currencyConverter = converter;
        }

        public override async Task<Guid> ExecuteCommand(SaveCartCommand command, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(command.CustomerId, cancellationToken);
            var product = await _unitOfWork.ProductRepository.GetById(command.Product.Id, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (product == null)
                throw new InvalidDataException("Product not found.");

            var cart = await _unitOfWork.CartRepository.GetByCustomerId(customer.Id, cancellationToken);
            var quantity = command.Product.Quantity;

            if (cart == null)
            {
                cart = new Cart(Guid.NewGuid(), customer);
                cart.AddItem(product, quantity);
                await _unitOfWork.CartRepository.Add(cart, cancellationToken);
            }
            else
            {
               cart.ChangeCart(product, quantity);
            }

            await _unitOfWork.CommitAsync();
            return cart.Id;
        }
    }
}
