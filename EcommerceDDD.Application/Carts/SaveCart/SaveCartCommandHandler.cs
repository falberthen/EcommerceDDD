using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Carts;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

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
            var customerId = CustomerId.Of(command.CustomerId);
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(customerId, cancellationToken);
            var productId = ProductId.Of(command.Product.Id);
            var product = await _unitOfWork.ProductRepository.GetProductById(productId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (product == null)
                throw new InvalidDataException("Product not found.");


            var cart = await _unitOfWork.CartRepository.GetCartByCustomerId(customerId, cancellationToken);
            var quantity = command.Product.Quantity;
            var cartItemProductData = new CartItemProductData(product.Id, product.Price, quantity);

            if (cart == null)
            {
                var cartId = CartId.Of(Guid.NewGuid());
                cart = new Cart(cartId, customerId);
                cart.AddItem(cartItemProductData);
                await _unitOfWork.CartRepository.AddCart(cart, cancellationToken);
            }
            else
            {
               cart.ChangeCart(cartItemProductData);
            }

            await _unitOfWork.CommitAsync();
            return cart.Id.Value;
        }
    }
}
