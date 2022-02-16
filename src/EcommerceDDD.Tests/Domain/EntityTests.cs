using Xunit;
using FluentAssertions;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Tests.Domain;

public class EntityTests
{

    [Fact]
    public void Two_entities_are_always_different()
    {
        var money = Money.Of(10, Currency.USDollar.Code);
        var productName = "Product X";

        var productX = Product.CreateNew(productName, money);
        var productY = Product.CreateNew(productName, money);

        (productX.GetHashCode() == productY.GetHashCode()).Should().BeFalse();
        productX.Equals(productY).Should().BeFalse();
    }
}