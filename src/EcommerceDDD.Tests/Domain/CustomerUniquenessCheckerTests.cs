using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.SeedWork;
using FluentAssertions;
using NSubstitute;
using System;
using Xunit;

namespace EcommerceDDD.Tests
{
    public class CustomerUniquenessCheckerTests
    {
        const string name = "Customer";
        const string email = "test@email.com";        

        [Fact]
        public void Customer_email_is_available()
        {
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(email).Returns(true);

            var customer = Customer.CreateNew(email, name, customerUniquenessChecker);

            customer.Email.Should().Be(email);
        }

        [Fact]
        public void Customer_email_is_already_in_use()
        {
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            customerUniquenessChecker.IsUserUnique(email).Returns(false);

            Action action = () => 
                Customer.CreateNew(email, name, customerUniquenessChecker);

            action.Should().Throw<BusinessRuleException>();
        }
    }
}