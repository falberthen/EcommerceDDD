using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Domain.Carts;
using System;

namespace EcommerceDDD.Application.Carts.GetCartDetails
{
    public class GetCartQueryHandler : QueryHandler<GetCartDetailsQuery, CartDetailsViewModel>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _currencyConverter;

        public GetCartQueryHandler(IEcommerceUnitOfWork unitOfWork, ICurrencyConverter currencyConverter)
        {
            _unitOfWork = unitOfWork;
            _currencyConverter = currencyConverter;
        }

        public async override Task<CartDetailsViewModel> ExecuteQuery(GetCartDetailsQuery query, CancellationToken cancellationToken)
        {
            CartDetailsViewModel viewModel = new CartDetailsViewModel();

            var customer = await _unitOfWork.CustomerRepository.GetById(query.CustomerId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (string.IsNullOrWhiteSpace(query.Currency))
                throw new InvalidDataException("Currency can't be empty.");

            var cart = await _unitOfWork.CartRepository.GetByCustomerId(query.CustomerId, cancellationToken);

            if (cart == null)
            {
                // Creating cart
                cart = new Cart(Guid.NewGuid(), customer);
                await _unitOfWork.CartRepository.Add(cart, cancellationToken);
                await _unitOfWork.CommitAsync();
            }                

            var currency = Currency.FromCode(query.Currency);

            viewModel.CartId = cart.Id;
            if (cart.Items.Count > 0)
            {
                var productIds = cart.Items.Select(p => p.Product.Id).ToList();
                var products = await _unitOfWork.ProductRepository.GetByIds(productIds, cancellationToken);

                if (products == null)
                    throw new InvalidDataException("Products not found");

                foreach (var cartItem in cart.Items)
                {
                    var product = products.Single(p => p.Id == cartItem.Product.Id);

                    viewModel.CartItems.Add(new CartItemDetailsViewModel
                    {
                        ProductId = cartItem.Product.Id,
                        ProductQuantity = cartItem.Quantity,
                        ProductName = product.Name,
                        ProductPrice = _currencyConverter.Convert(currency, cartItem.Product.Price).Value,
                        CurrencySymbol = currency.Symbol,
                    });
                }

                viewModel.CalculateTotalOrderPrice();
            }

            return viewModel;
        }
    }
}
