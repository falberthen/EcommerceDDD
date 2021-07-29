using EcommerceDDD.Application.Carts;
using EcommerceDDD.Application.Carts.CreateCart;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EcommerceDDD.Tests.Commands
{
    public class SaveCartCommandHandlerTests
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICustomers _customers;
        private readonly IProducts _products;
        private readonly ICarts _cartRepository;
        private readonly ICurrencyConverter _currencyConverter;

        public SaveCartCommandHandlerTests()
        {
            _customers = NSubstitute.Substitute.For<ICustomers>();
            _products = NSubstitute.Substitute.For<IProducts>();
            _cartRepository = NSubstitute.Substitute.For<ICarts>();
            _currencyConverter = Substitute.For<ICurrencyConverter>();
            _unitOfWork = NSubstitute.Substitute.For<IEcommerceUnitOfWork>();

            _unitOfWork.Customers.ReturnsForAnyArgs(_customers);
            _unitOfWork.Products.ReturnsForAnyArgs(_products);
            _unitOfWork.Carts.ReturnsForAnyArgs(_cartRepository);
        }

        [Fact]
        public async Task Cart_has_been_created_for_costumer()
        {
            var currency = Currency.USDollar;
            var productPrice = 12.5;
            var productQuantity = 10;
            var customerEmail = "test@domain.com";

            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(customerEmail).Returns(true);

            var productMoney = Money.Of(Convert.ToDecimal(productPrice), currency.Code);
            var customer = Customer
                .CreateCustomer(customerEmail, "Customer X", customerUniquenessChecker);

            _customers
                .GetById(Arg.Any<CustomerId>()).Returns(customer);

            var product = Product.CreateNew("Product X", productMoney);
            _products.GetById(Arg.Any<ProductId>()).Returns(product);

            var handler = new SaveCartCommandHandler(_unitOfWork);
            var command = new SaveCartCommand(customer.Id.Value, new ProductDto(product.Id.Value, productQuantity));
            var result = await handler.Handle(command, CancellationToken.None);

            await _cartRepository.Received(1)
                .Add(Arg.Is((Cart c) => c.Id.Value == result.Id), Arg.Any<CancellationToken>());

            await _customers.Received(1)
                .GetById(customer.Id, Arg.Any<CancellationToken>());

            await _unitOfWork.Received(1)
                .CommitAsync(Arg.Any<CancellationToken>());

            result.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task SaveCartCommand_validation_should_fail_with_empty_required_fields()
        {
            var handler = new SaveCartCommandHandler(_unitOfWork);
            var command = new SaveCartCommand(Guid.Empty, null);
            var result = await handler.Handle(command, CancellationToken.None);

            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Count.Should().Be(2);
        }
    }
}
