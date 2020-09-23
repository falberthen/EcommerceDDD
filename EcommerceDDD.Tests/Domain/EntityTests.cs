using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;
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

            var product = new Product(Guid.NewGuid(), "Product X", Money.Of(10, "USD"));
            var customer = Customer.CreateCustomer(Guid.NewGuid(), email, "Customer X", customerUniquenessChecker);

            (product.GetHashCode() == customer.GetHashCode()).Should().BeFalse();
            product.Equals(customer).Should().BeFalse();
        }

        [Fact]
        public void Entities_are_equal_with_same_ids()
        {
            var money = Money.Of(10, "USD");
            var productName = "Product X";
            var id = Guid.NewGuid();

            var productX = new Product(id, productName, money);
            var productY = new Product(id, "Product Y", money);

            (productX.GetHashCode() == productY.GetHashCode()).Should().BeTrue();
            productX.Equals(productY).Should().BeTrue();
        }

        [Fact]
        public void Entities_arent_equal_with_different_ids()
        {
            var money = Money.Of(10, "USD");
            var productName = "Product X";

            var productX = new Product(Guid.NewGuid(), productName, money);
            var productY = new Product(Guid.NewGuid(), productName, money);

            (productX.GetHashCode() == productY.GetHashCode()).Should().BeFalse();
            productX.Equals(productY).Should().BeFalse();
        }
    }
}
