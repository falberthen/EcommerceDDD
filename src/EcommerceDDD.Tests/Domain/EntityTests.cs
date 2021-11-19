using Xunit;
using NSubstitute;
using FluentAssertions;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Tests.Domain;

public class EntityTests
{
    [Fact]
    public void Entities_arent_equal_with_different_types()
    {
        var email = "email@domain.com";
        var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
        customerUniquenessChecker.IsUserUnique(email).Returns(true);

        var product = Product.CreateNew("Product X", Money.Of(10, Currency.USDollar.Code));
        var customer = Customer.CreateNew(email, "Customer X", customerUniquenessChecker);

        (product.GetHashCode() == customer.GetHashCode()).Should().BeFalse();
        product.Equals(customer).Should().BeFalse();
    }


    [Fact]
    public void Entities_arent_equal_with_different_ids()
    {
        var money = Money.Of(10, Currency.USDollar.Code);
        var productName = "Product X";

        var productX = Product.CreateNew(productName, money);
        var productY = Product.CreateNew(productName, money);

        (productX.GetHashCode() == productY.GetHashCode()).Should().BeTrue();
        productX.Equals(productY).Should().BeFalse();
    }
}