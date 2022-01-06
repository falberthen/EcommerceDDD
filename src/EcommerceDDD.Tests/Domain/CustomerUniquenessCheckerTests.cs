using Xunit;
using System;
using NSubstitute;
using FluentAssertions;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.SeedWork;
using System.Threading.Tasks;

namespace EcommerceDDD.Tests;

public class CustomerUniquenessCheckerTests
{
    const string name = "Customer";
    const string email = "test@email.com";        

    [Fact]
    public async Task Customer_email_is_available()
    {
        var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
        customerUniquenessChecker.IsUserUnique(email).Returns(true);

        var customer = await Customer.CreateNew(email, name, customerUniquenessChecker);

        customer.Email.Should().Be(email);
    }

    [Fact]
    public void Customer_email_is_already_in_use()
    {
        var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
        customerUniquenessChecker.IsUserUnique(email).Returns(false);

        Func<Task> action = async () =>
            await Customer.CreateNew(email, name, customerUniquenessChecker);

        action.Should().ThrowAsync<BusinessRuleException>();
    }
}