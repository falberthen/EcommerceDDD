using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;
using System;
using Xunit;

namespace EcommerceDDD.Tests.Domain
{
    public class EntityTests
    {
        [Fact]
        public void Entities_arent_equal_with_different_types()
        {
            var email = "email@domain.com";
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(email).Returns(true);

            var productId = ProductId.Of(Guid.NewGuid());
            var product = new Product(productId, "Product X", Money.Of(10, Currency.USDollar.Code));
            var customer = Customer.CreateCustomer(email, "Customer X", customerUniquenessChecker);

            (product.GetHashCode() == customer.GetHashCode()).Should().BeFalse();
            product.Equals(customer).Should().BeFalse();
        }

        [Fact]
        public void Entities_are_equal_with_same_ids()
        {
            var money = Money.Of(10, Currency.USDollar.Code);
            var productName = "Product X";
            var id = Guid.NewGuid();

            var productId = ProductId.Of(Guid.NewGuid());
            var productX = new Product(productId, productName, money);
            var productY = new Product(productId, "Product Y", money);

            (productX.GetHashCode() == productY.GetHashCode()).Should().BeTrue();
            productX.Equals(productY).Should().BeTrue();
        }

        [Fact]
        public void Entities_arent_equal_with_different_ids()
        {
            var money = Money.Of(10, Currency.USDollar.Code);
            var productName = "Product X";

            var productX = new Product(ProductId.Of(Guid.NewGuid()), productName, money);
            var productY = new Product(ProductId.Of(Guid.NewGuid()), productName, money);

            (productX.GetHashCode() == productY.GetHashCode()).Should().BeTrue();
            productX.Equals(productY).Should().BeFalse();
        }
    }
}
