using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.SharedKernel;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Domain.Carts;
using System;
using EcommerceDDD.Domain.Customers;

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

            var customerId = CustomerId.Of(query.CustomerId);
            var customer = await _unitOfWork.Customers
                .GetById(customerId, cancellationToken);

            if (customer == null)
                throw new InvalidDataException("Customer not found.");

            if (string.IsNullOrWhiteSpace(query.Currency))
                throw new InvalidDataException("Currency can't be empty.");

            var cart = await _unitOfWork.Carts
                .GetByCustomerId(customerId, cancellationToken);

            if (cart == null)
            {
                // Creating cart
                cart = Cart.CreateNew(customerId);
                await _unitOfWork.Carts
                    .Add(cart, cancellationToken);

                await _unitOfWork.CommitAsync();
            }                

            var currency = Currency.FromCode(query.Currency);

            viewModel.CartId = cart.Id.Value;
            if (cart.Items.Count > 0)
            {
                var productIds = cart.Items.Select(p => p.ProductId).ToList();
                var products = await _unitOfWork.Products
                    .GetByIds(productIds, cancellationToken);

                if (products == null)
                    throw new InvalidDataException("Products not found");

                foreach (var cartItem in cart.Items)
                {
                    var product = products.Single(p => p.Id == cartItem.ProductId);
                    var convertedPrice = _currencyConverter.Convert(currency, product.Price);
                    viewModel.CartItems.Add(new CartItemDetailsViewModel
                    {                        
                        ProductId = cartItem.ProductId.Value,
                        ProductQuantity = cartItem.Quantity,
                        ProductName = product.Name,
                        ProductPrice = Math.Round(convertedPrice.Value, 2),
                        CurrencySymbol = currency.Symbol,
                    }); ;
                }

                viewModel.CalculateTotalOrderPrice();
            }

            return viewModel;
        }
    }
}
