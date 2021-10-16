using EcommerceDDD.Application.Orders.PlaceOrder;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EcommerceDDD.Tests.Application
{
    public class PlaceOrderCommandHandlerTests
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly ICustomers _customers;
        private readonly IProducts _products;
        private readonly IQuotes _quotes;
        private readonly ICurrencyConverter _currencyConverter;

        public PlaceOrderCommandHandlerTests()
        {
            _customers = NSubstitute.Substitute.For<ICustomers>();
            _products = NSubstitute.Substitute.For<IProducts>();
            _quotes = NSubstitute.Substitute.For<IQuotes>();
            _currencyConverter = Substitute.For<ICurrencyConverter>();
            _unitOfWork = NSubstitute.Substitute.For<IEcommerceUnitOfWork>();

            _unitOfWork.Customers.ReturnsForAnyArgs(_customers);
            _unitOfWork.Products.ReturnsForAnyArgs(_products);
            _unitOfWork.Quotes.ReturnsForAnyArgs(_quotes);
        }

        [Fact]
        public async Task Order_has_been_placed_for_customer()
        {
            var currency = Currency.CanadianDollar;
            var productPrice = 12.5;
            var productQuantity = 10;
            var customerEmail = "test@domain.com";

            var productMoney = Money.Of(Convert.ToDecimal(productPrice), currency.Code);
            _currencyConverter.Convert(currency, Money.Of(Convert.ToDecimal(productPrice * productQuantity), currency.Code))
                .Returns(productMoney);

            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(customerEmail).Returns(true);

            var customerId = CustomerId.Of(Guid.NewGuid());
            var customer = Customer.CreateNew(customerEmail, "Customer X", customerUniquenessChecker);

            _customers.GetById(Arg.Any<CustomerId>()).Returns(customer);

            var product = Product.CreateNew("Product X", productMoney);
            _products.GetById(Arg.Any<ProductId>()).Returns(product);

            var productData = new QuoteItemProductData(product.Id, product.Price, productQuantity);
            var quote = Quote.CreateNew(customerId);
            quote.AddItem(productData);

            List<Product> products = new List<Product>() { product };
            _quotes.GetById(quote.Id).Returns(quote);
            _products.GetByIds(Arg.Any<List<ProductId>>()).Returns(products);

            var placeOrderCommandHandler = new PlaceOrderCommandHandler(_unitOfWork, _currencyConverter);
            var placeOrderCommand = new PlaceOrderCommand(quote.Id.Value, customerId.Value, currency.Code);

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
