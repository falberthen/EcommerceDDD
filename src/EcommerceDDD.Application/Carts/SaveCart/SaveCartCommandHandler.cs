using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Carts;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;

namespace EcommerceDDD.Application.Carts.CreateCart
{
    public class SaveCartCommandHandler : CommandHandler<SaveCartCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public SaveCartCommandHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Guid> ExecuteCommand(SaveCartCommand command, 
            CancellationToken cancellationToken)
        {
            var customerId = CustomerId.Of(command.CustomerId);
            var customer = await _unitOfWork.Customers
                .GetById(customerId, cancellationToken);

            var productId = ProductId.Of(command.Product.Id);
            var product = await _unitOfWork.Products
                .GetById(productId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (product == null)
                throw new InvalidDataException("Product not found.");

            var cart = await _unitOfWork.Carts.
                GetByCustomerId(customerId, cancellationToken);

            var quantity = command.Product.Quantity;
            var cartItemProductData = new CartItemProductData(
                product.Id, 
                product.Price, quantity
            );

            if (cart == null)
            {
                cart = Cart.CreateNew(customerId);
                cart.AddItem(cartItemProductData);
                await _unitOfWork.Carts
                    .Add(cart, cancellationToken);
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
