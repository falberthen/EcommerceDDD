using EcommerceDDD.Application.Orders.PlaceOrder;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EcommerceDDD.Tests.Commands
{
    public class PlaceOrderCommandHandlerTests
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICurrencyConverter _currencyConverter;

        public PlaceOrderCommandHandlerTests()
        {
            _customerRepository = NSubstitute.Substitute.For<ICustomerRepository>();
            _productRepository = NSubstitute.Substitute.For<IProductRepository>();
            _cartRepository = NSubstitute.Substitute.For<ICartRepository>();
            _currencyConverter = Substitute.For<ICurrencyConverter>();
            _unitOfWork = NSubstitute.Substitute.For<IEcommerceUnitOfWork>();

            _unitOfWork.CustomerRepository.ReturnsForAnyArgs(_customerRepository);
            _unitOfWork.ProductRepository.ReturnsForAnyArgs(_productRepository);
            _unitOfWork.CartRepository.ReturnsForAnyArgs(_cartRepository);
        }

        [Fact]
        public async Task Order_has_been_placed_for_customer()
        {                                    
            var currency = Currency.USDollar;
            var productPrice = 12.5;
            var productQuantity = 10;
            var customerEmail = "test@domain.com";

            var productMoney = Money.Of(Convert.ToDecimal(productPrice), currency.Code);
            _currencyConverter.Convert(currency, Money.Of(Convert.ToDecimal(productPrice * productQuantity), currency.Code))
                .Returns(productMoney);

            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(customerEmail).Returns(true);

            var customer = Customer.CreateCustomer(Guid.NewGuid(), customerEmail, "Customer X", customerUniquenessChecker);
            _customerRepository.GetById(Arg.Any<Guid>()).Returns(customer);

            var product = new Product(Guid.NewGuid(), "Product X", productMoney);
            _productRepository.GetById(Arg.Any<Guid>()).Returns(product);

            var cart = new Cart(Guid.NewGuid(), customer);
            cart.AddItem(product, productQuantity);
            _cartRepository.GetById(cart.Id).Returns(cart);

            var placeOrderCommandHandler = new PlaceOrderCommandHandler(_unitOfWork, _currencyConverter);
            var placeOrderCommand = new PlaceOrderCommand(cart.Id, customer.Id, currency.Code);
            var orderResult = await placeOrderCommandHandler.Handle(placeOrderCommand, CancellationToken.None);

            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
            orderResult.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task PlaceOrderCommand_validation_should_fail_with_empty_required_fields()
        {
            var handler = new PlaceOrderCommandHandler(_unitOfWork, _currencyConverter);
            var command = new PlaceOrderCommand(Guid.Empty, Guid.Empty, string.Empty);
            var result = await handler.Handle(command, CancellationToken.None);

            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Count.Should().Be(3);
        }
    }
}
