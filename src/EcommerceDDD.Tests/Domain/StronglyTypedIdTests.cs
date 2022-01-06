using Xunit;
using System;
using FluentAssertions;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;

namespace EcommerceDDD.Tests.Domain;

public class StronglyTypedIdTests
{
    [Fact]
    public void Id_cannot_be_empty()
    {
        var ex = Assert.Throws<BusinessRuleException>(() => new CustomerId(new Guid()));
        ex.GetType().Should().Be(typeof(BusinessRuleException));
    }

    [Fact]
    public void Two_ids_with_same_value_and_type_are_equal()
    {
        var guidValue = Guid.NewGuid();

        var customerId = new CustomerId(guidValue);
        var customerId2 = new CustomerId(guidValue);

        var areEqual = customerId == customerId2;

        areEqual.Should().BeTrue();
        customerId.Should().Be(customerId2);
    }

    [Fact]
    public void Two_ids_with_same_value_and_diff_types_are_different()
    {
        var guidValue = Guid.NewGuid();

        var customerId = new CustomerId(guidValue);
        var orderId = new OrderId(guidValue);

        customerId.Should().NotBe(orderId);
    }
}