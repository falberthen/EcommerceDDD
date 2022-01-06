using System;
using Xunit;
using NSubstitute;
using FluentAssertions;
using System.Threading;
using EcommerceDDD.Domain;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Quotes;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Application.Quotes;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Application.Quotes.SaveQuote;
using EcommerceDDD.Application.Quotes.ChangeQuote;

namespace EcommerceDDD.Tests.Commands;

public class SaveQuoteCommandHandlerTests
{
    private readonly IEcommerceUnitOfWork _unitOfWork;
    private readonly ICustomers _customers;
    private readonly IProducts _products;
    private readonly IQuotes _quotes;

    public SaveQuoteCommandHandlerTests()
    {
        _customers = NSubstitute.Substitute.For<ICustomers>();
        _products = NSubstitute.Substitute.For<IProducts>();
        _quotes = NSubstitute.Substitute.For<IQuotes>();
        _unitOfWork = NSubstitute.Substitute.For<IEcommerceUnitOfWork>();

        _unitOfWork.Customers.ReturnsForAnyArgs(_customers);
        _unitOfWork.Products.ReturnsForAnyArgs(_products);
        _unitOfWork.Quotes.ReturnsForAnyArgs(_quotes);
    }

    [Fact]
    public async Task Quote_has_been_created_for_costumer()
    {
        var currency = Currency.USDollar;
        var productPrice = 12.5;
        var productQuantity = 10;
        var customerEmail = "test@domain.com";

        var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
        customerUniquenessChecker.IsUserUnique(customerEmail).Returns(true);

        var productMoney = Money.Of(Convert.ToDecimal(productPrice), currency.Code);
        var customer = await Customer
            .CreateNew(customerEmail, "Customer X", customerUniquenessChecker);

        _customers
            .GetById(Arg.Any<CustomerId>()).Returns(customer);

        var product = Product.CreateNew("Product X", productMoney);
        _products.GetById(Arg.Any<ProductId>()).Returns(product);

        var handler = new CreateQuoteCommandHandler(_unitOfWork);
        var command = new CreateQuoteCommand(customer.Id.Value, new ProductDto(product.Id.Value, productQuantity));
        var result = await handler.Handle(command, CancellationToken.None);

        await _quotes.Received(1)
            .Add(Arg.Is((Quote c) => c.Id.Value == result.Id), Arg.Any<CancellationToken>());

        await _customers.Received(1)
            .GetById(customer.Id, Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task SaveQuoteCommand_validation_should_fail_with_empty_required_fields()
    {
        var handler = new ChangeQuoteCommandHandler(_unitOfWork);
        var command = new ChangeQuoteCommand(Guid.Empty, null);
        var result = await handler.Handle(command, CancellationToken.None);

        result.ValidationResult.IsValid.Should().BeFalse();
        result.ValidationResult.Errors.Count.Should().Be(2);
    }
}